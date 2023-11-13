// from https://github.com/csdvrx/derasterize

/*bin/echo  ' -*- mode:c;indent-tabs-mode:nil;c-basic-offset:2;coding:utf-8 -*-│
│vi: set net ft=c ts=2 sts=2 sw=2 fenc=utf-8                                :vi│
╞══════════════════════════════════════════════════════════════════════════════╡
│ Copyright 2019 Csdvrx & Justine Alexandra Roberts Tunney                     │
│                                                                              │
│ Permission to use, copy, modify, and/or distribute this software for any     │
│ purpose with or without fee is hereby granted, provided that the above       │
│ copyright notice and this permission notice appear in all copies.            │
│                                                                              │
│ THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES     │
│ WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF             │
│ MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR      │
│ ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES       │
│ WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN        │
│ ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF      │
│ OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.               │
╞══════════════════════════════════════════════════════════════════════════════╡
│ To use this, install a compiler and (for now) imagemagick:                   │
│  apt install build-essential imagemagick                                     │
│                                                                              │
│ Then make this file executable or run it with sh:                            │
│  chmod +x derasterize.c                                                      │
│  sh derasterize.c -y10 -x10 samples/wave.png                                 │
│                                                                              │
│ You can use the c file directly: the blub below recompiles when needed.      │
╚────────────────────────────────────────────────────────────────────'>/dev/null
  if ! [ "${0%.*}" -nt "$0" ]; then
    if [ -z "$CC" ]; then
      CC=$(command -v clang-9) ||
      CC=$(command -v clang-8) ||
      CC=$(command -v clang) ||
      CC=$(command -v gcc) ||
      CC=$(command -v cc)
    fi
    COPTS="-g -march=native -Ofast"
    $CC $COPTS -o "${0%.*}" "$0" -lm || exit
  fi
  exec ./"${0%.*}" "$@"
  exit

*/

#define HELPTEXT "\n\
NAME\n\
\n\
  derasterize - convert pictures to text using unicode ANSI art\n\
\n\
SYNOPSIS\n\
\n\
  derasterize [PNG|JPG|ETC]...\n\
\n\
DESCRIPTION\n\
\n\
  This program converts pictures into unicode text and ANSI colors so\n\
  that images can be displayed within a terminal. It performs lots of\n\
  AVX2 optimized math to deliver the best quality on modern terminals\n\
  with 24-bit color support, e.g. Kitty, Gnome Terminal, CMD.EXE, etc\n\
\n\
  The default output if fullscreen but can be changed:\n\
  -xX\n\
          If X is positive, hardcode the width in caracters to X\n\
          If X is negative, remove as much from the fullscreen width\n\
  -y\n\
          If Y is positive, hardcode the height in caracters to Y\n\
          If Y is negative, remove as much from the fullscreen height\n\
\n\
EXAMPLES\n\
\n\
  $ ./derasterize.c samples/wave.png > wave.uaart\n\
  $ cat wave.uaart\n\
\n\
AUTHORS\n\
\n\
  Csdvrx <csdvrx@outlook.com>\n\
  Justine Tunney <jtunney@gmail.com>\n\
"

#ifdef __ELF__
__asm__(".ident\t\"\\n\\n\
derasterize (ISC License)\\n\
Copyright 2019 Csdvrx & Justine Alexandra Roberts Tunney\"");
#endif

#include <limits>

#include <fcntl.h>
#include <fenv.h>
#include <limits.h>
#include <locale.h>
#include <malloc.h>
#include <math.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
// Can be missing in msys2
#include <uchar.h>

#include "lib.h"

#define BEST 0
#define FAST 1
#define FASTER 2

#ifndef MODE
#ifdef __AVX2__
#define MODE BEST
#else
#define MODE FAST
#endif
#endif

// TODO: we should make that runtime choices, with separate command line options for MC and GN

#if MODE == BEST
#define MC 9u  /* log2(#)o of color cmbos to consider */
// FIXME: shouldn't that be 44 in best? Or maybe declare mode A, mode B, etc.
#define GN 35u /* # of glyphs to consider */
#elif MODE == FAST
#define MC 6u
#define GN 35u
#elif MODE == FASTER
#define MC 4u
#define GN 25u
#endif

#define FLOAT float
#define FLOAT_C(X) X##f
#define CN 3u        /* # channels (rgb) */
#define YS 8u        /* row stride -or- block height */
#define XS 4u        /* column stride -or- block width */
#define GT 44u       /* total glyphs */
#define BN (YS * XS) /* # scalars in block/glyph plane */

#define PHIPRIME 0x9E3779B1u
#define SQR(X) ((X) * (X))
#define ABS(X) FLOAT_C(fabs)(X)
#define SQRT(X) FLOAT_C(sqrt)(X)
#define MOD(X, Y) FLOAT_C(fmod)(X, Y)
#define MIN(x, y) ((x) < (y) ? (x) : (y))
#define MAX(x, y) ((x) > (y) ? (x) : (y))

#define ARRAYLEN(A) \
  ((sizeof(A) / sizeof(*(A))) / ((unsigned)!(sizeof(A) % sizeof(*(A)))))

#define ORDIE(X)          \
  do                      \
    if (!(X)) {           \
      exit(EXIT_FAILURE); \
    }                     \
  while (0)

// The modes below use various unicodes for 'progresssive' pixelization:
// each mode supersets the previous to increase resolution more and more.
// Ideally, a fully dense mapping of the (Y*X) space defined by kGlyph size
// would produce a picture perfect image, but at the cost of sampling speed.
// Therefore, supersets are parcimonious: they only add the minimal set of
// missing shapes that can increase resolution.
// Ideally, this should be based on a study of the residual, but some logic
// can go a long way: after some block pixelization, will need diagonals
// FIXME: then shouldn't box drawing go right after braille?

// TODO: explain the differences between each mode:
// Mode A is full, empty, half blocks top and bottom: , █,▄,▀
// Mode B superset: with quadrants:  ▐,▌,▝,▙,▗,▛,▖,▜,▘,▟,▞,▚,
// Mode C superset: with fractional eights along X and Y
//  _,▁,▂,▃,▄,▅,▆,▇ :█:▉,▊,▋,▌,▍,▎,▏
// Mode X use box drawing, mode X use diagonal blocks, mode X use braille etc

#define W(B, S) B##U << S
#define G(AA, AB, AC, AD, BA, BB, BC, BD, CA, CB, CC, CD, DA, DB, DC, DD, EA, \
          EB, EC, ED, FA, FB, FC, FD, GA, GB, GC, GD, HA, HB, HC, HD)         \
  (W(AA, 000) | W(AB, 001) | W(AC, 002) | W(AD, 003) | W(BA, 004) |           \
   W(BB, 005) | W(BC, 006) | W(BD, 007) | W(CA, 010) | W(CB, 011) |           \
   W(CC, 012) | W(CD, 013) | W(DA, 014) | W(DB, 015) | W(DC, 016) |           \
   W(DD, 017) | W(EA, 020) | W(EB, 021) | W(EC, 022) | W(ED, 023) |           \
   W(FA, 024) | W(FB, 025) | W(FC, 026) | W(FD, 027) | W(GA, 030) |           \
   W(GB, 031) | W(GC, 032) | W(GD, 033) | W(HA, 034) | W(HB, 035) |           \
   W(HC, 036) | W(HD, 037))


// The glyph size it set by the resolution of the most precise mode, ex:
// - Mode C: along the X axis, need >= 8 steps for the 8 fractional width
// FIXME: now we can only use 4 chars instead of the extra ▉,▊,▋,▌,▍,▎,▏
//
// - Mode X: along the Y axis, need >= 8 steps to separate the maximal 6 dots
// from the space left below, seen by overimposing an underline  ⠿_ 
// along the 3 dots, the Y axis is least 1,0,1,0,1,0,0,1 so 8 steps
//
// Problem: fonts are taller than wider, and terminals are tradionally 80x24, so
// - we shouldn't use square glyphs, 8x16 seems to be the minimal size
// - we should adapt the conversion to BMP to avoid accidental Y downsampling

static const uint32_t kGlyphs[GT] = /* clang-format off */{
    /* U+0020 ' ' empty block [ascii:20,cp437:20] */
    G(0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0),
      /* U+2588 '█' full block [cp437] */
      G(1,1,1,1,
        1,1,1,1,
        1,1,1,1,
        1,1,1,1,
        1,1,1,1,
        1,1,1,1,
        1,1,1,1,
        1,1,1,1),
        /* U+2584 '▄' lower half block [cp437:dc] */
        G(0,0,0,0,
          0,0,0,0,
          0,0,0,0,
          0,0,0,0,
          1,1,1,1,
          1,1,1,1,
          1,1,1,1,
          1,1,1,1),
          /* U+2580 '▀' upper half block [cp437] */
          G(1,1,1,1,
            1,1,1,1,
            1,1,1,1,
            1,1,1,1,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0),
            // Mode B
            /* U+2590 '▐' right half block [cp437:de] */
            G(0,0,1,1,
              0,0,1,1,
              0,0,1,1,
              0,0,1,1,
              0,0,1,1,
              0,0,1,1,
              0,0,1,1,
              0,0,1,1),
              /* U+258c '▌' left half block [cp437] */
              G(1,1,0,0,
                1,1,0,0,
                1,1,0,0,
                1,1,0,0,
                1,1,0,0,
                1,1,0,0,
                1,1,0,0,
                1,1,0,0),
                /* U+259d '▝' quadrant upper right */
                G(0,0,1,1,
                  0,0,1,1,
                  0,0,1,1,
                  0,0,1,1,
                  0,0,0,0,
                  0,0,0,0,
                  0,0,0,0,
                  0,0,0,0),
                  /* U+2599 '▙' quadrant upper left and lower left and lower right */
                  G(1,1,0,0,
                    1,1,0,0,
                    1,1,0,0,
                    1,1,0,0,
                    1,1,1,1,
                    1,1,1,1,
                    1,1,1,1,
                    1,1,1,0),
                    /* U+2597 '▗' quadrant lower right */
                    G(0,0,0,0,
                      0,0,0,0,
                      0,0,0,0,
                      0,0,0,0,
                      0,0,1,1,
                      0,0,1,1,
                      0,0,1,1,
                      0,0,1,1),
                      /* U+259b '▛' quadrant upper left and upper right and lower left */
                      G(1,1,1,1,
                        1,1,1,1,
                        1,1,1,1,
                        1,1,1,1,
                        1,1,0,0,
                        1,1,0,0,
                        1,1,0,0,
                        1,1,0,1),
                        /* U+2596 '▖' quadrant lower left */
                        G(0,0,0,0,
                          0,0,0,0,
                          0,0,0,0,
                          0,0,0,0,
                          1,1,0,0,
                          1,1,0,0,
                          1,1,0,0,
                          1,1,0,0),
                          /* U+259c '▜' quadrant upper left and upper right and lower right */
                          G(1,1,1,1,
                            1,1,1,1,
                            1,1,1,1,
                            1,1,1,1,
                            0,0,1,1,
                            0,0,1,1,
                            0,0,1,1,
                            0,0,1,0),
                            /* U+2598 '▘' quadrant upper left */
                            G(1,1,0,0,
                              1,1,0,0,
                              1,1,0,0,
                              1,1,0,0,
                              0,0,0,0,
                              0,0,0,0,
                              0,0,0,0,
                              0,0,0,0),
                              /* U+259F '▟' quadrant upper right and lower left and lower right */
                              G(0,0,1,1,
                                0,0,1,1,
                                0,0,1,1,
                                0,0,1,1,
                                1,1,1,1,
                                1,1,1,1,
                                1,1,1,1,
                                1,1,1,0),
                                /* U+259e '▞' quadrant upper right and lower left */
                                G(0,0,1,1,
                                  0,0,1,1,
                                  0,0,1,1,
                                  0,0,1,1,
                                  1,1,0,0,
                                  1,1,0,0,
                                  1,1,0,0,
                                  1,1,0,0),
                                  /* U+259a '▚' quadrant upper left and lower right */
                                  G(1,1,0,0,
                                    1,1,0,0,
                                    1,1,0,0,
                                    1,1,0,0,
                                    0,0,1,1,
                                    0,0,1,1,
                                    0,0,1,1,
                                    0,0,1,0),
                                    // Mode C
                                    /* U+2594 '▔' upper one eighth block */
                                    G(1,1,1,1,
                                      0,0,0,0,
                                      0,0,0,0,
                                      0,0,0,0,
                                      0,0,0,0,
                                      0,0,0,0,
                                      0,0,0,0,
                                      0,0,0,0),
                                      /* U+2581 '▁' lower one eighth block */
                                      G(0,0,0,0,
                                        0,0,0,0,
                                        0,0,0,0,
                                        0,0,0,0,
                                        0,0,0,0,
                                        0,0,0,0,
                                        0,0,0,0,
                                        1,1,1,1),
                                        /* U+2582 '▂' lower one quarter block */
                                        G(0,0,0,0,
                                          0,0,0,0,
                                          0,0,0,0,
                                          0,0,0,0,
                                          0,0,0,0,
                                          0,0,0,0,
                                          1,1,1,1,
                                          1,1,1,1),
                                          /* U+2583 '▃' lower three eighths block */
                                          G(0,0,0,0,
                                            0,0,0,0,
                                            0,0,0,0,
                                            0,0,0,0,
                                            0,0,0,0,
                                            1,1,1,1,
                                            1,1,1,1,
                                            1,1,1,1),
                                            /* U+2585 '▃' lower five eighths block */
                                            G(0,0,0,0,
                                              0,0,0,0,
                                              0,0,0,0,
                                              1,1,1,1,
                                              1,1,1,1,
                                              1,1,1,1,
                                              1,1,1,1,
                                              1,1,1,1),
                                              /* U+2586 '▆' lower three quarters block */
                                              G(0,0,0,0,
                                                0,0,0,0,
                                                1,1,1,1,
                                                1,1,1,1,
                                                1,1,1,1,
                                                1,1,1,1,
                                                1,1,1,1,
                                                1,1,1,1),
                                                /* U+2587 '▇' lower seven eighths block */
                                                G(0,0,0,0,
                                                  1,1,1,1,
                                                  1,1,1,1,
                                                  1,1,1,1,
                                                  1,1,1,1,
                                                  1,1,1,1,
                                                  1,1,1,1,
                                                  1,1,1,1),
                                                  /* U+258e '▎' left one quarter block */
                                                  G(1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0,
                                                    1,0,0,0),
                                                    /* U+258a '▊' left three quarters block */
                                                    G(1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0),
                                                      /* ▁ *\
                                                    2501▕━▎box drawings heavy horizontal
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                   25019▕┉▎box drawings heavy quadruple dash horizontal
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,0,1,0,
                                                      0,1,0,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    2503▕┃▎box drawings heavy vertical
                                                      \* ▔ */
                                                    G(0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0),
                                                      /* ▁ *\
                                                    254b▕╋▎box drawings heavy vertical and horizontal
                                                      \* ▔ */
                                                    G(0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      1,1,1,1,
                                                      1,1,1,1,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0),
                                                      /* ▁ *\
                                                    2579▕╹▎box drawings heavy up
                                                      \* ▔ */
                                                    G(0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    257a▕╺▎box drawings heavy right
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,1,1,
                                                      0,0,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    257b▕╻▎box drawings heavy down
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0),
                                                      /* ▁ *\
                                                    2578▕╸▎box drawings heavy left
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,0,0,
                                                      1,1,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    250f▕┏▎box drawings heavy down and right
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,1,1,1,
                                                      0,1,1,1,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0),
                                                      /* ▁ *\
                                                    251b▕┛▎box drawings heavy up and left
                                                      \* ▔ */
                                                    G(0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    2513▕┓▎box drawings heavy down and left
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,0,
                                                      1,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0),
                                                      /* ▁ *\
                                                    2517▕┗▎box drawings heavy up and right
                                                      \* ▔ */
                                                    G(0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,0,
                                                      0,1,1,1,
                                                      0,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    25E2▕◢▎black lower right triangle
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,1,
                                                      0,0,1,1,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    25E3▕◣▎black lower left triangle
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,0,0,0,
                                                      1,1,0,0,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    25E4▕◥▎black upper right triangle
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      0,0,1,1,
                                                      0,0,0,1,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    25E5▕◤▎black upper left triangle
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      1,1,0,0,
                                                      1,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    2500▕═▎box drawings double horizontal
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    23BB▕⎻▎horizontal scan line 3
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0),
                                                      /* ▁ *\
                                                    23BD▕⎼▎horizontal scan line 9
                                                      \* ▔ */
                                                    G(0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      0,0,0,0,
                                                      1,1,1,1,
                                                      0,0,0,0,
                                                      0,0,0,0),
} /* clang-format on */;

static const char16_t kRunes[GT] = {
    u' ', /* 0020 empty block [ascii:20,cp437:20] */
    u'█', /* 2588 full block [cp437] */
    u'▄', /* 2584 lower half block [cp437:dc] */
    u'▀', /* 2580 upper half block [cp437] */
    u'▐', /* 2590 right half block [cp437:de] */
    u'▌', /* 258C left half block */
    u'▝', /* 259D quadrant upper right */
    u'▙', /* 2599 quadrant upper left and lower left and lower right */
    u'▗', /* 2597 quadrant lower right */
    u'▛', /* 259B quadrant upper left and upper right and lower left */
    u'▖', /* 2596 quadrant lower left */
    u'▜', /* 259C quadrant upper left and upper right and lower right */
    u'▘', /* 2598 quadrant upper left */
    u'▟', /* 259F quadrant upper right and lower left and lower right */
    u'▞', /* 259E quadrant upper right and lower left */
    u'▚', /* 259A quadrant upper left and lower right */
    u'▔', /* 2594 upper one eighth block */
    u'▁', /* 2581 lower one eighth block */
    u'▂', /* 2582 lower one quarter block */
    u'▃', /* 2583 lower three eighths block */
    u'▅', /* 2585 lower five eighths block */
    u'▆', /* 2586 lower three quarters block */
    u'▇', /* 2587 lower seven eighths block */
    u'▎', /* 258E left one quarter block */
    u'▊', /* 258A left three quarters block */
    u'━', /* 2501 box drawings heavy horizontal */
    u'┉', /* 2509 box drawings heavy quadruple dash horizontal */
    u'┃', /* 2503 box drawings heavy vertical */
    u'╋', /* 254B box drawings heavy vertical & horiz. */
    u'╹', /* 2579 box drawings heavy up */
    u'╺', /* 257A box drawings heavy right */
    u'╻', /* 257B box drawings heavy down */
    u'╸', /* 2578 box drawings heavy left */
    u'┏', /* 250F box drawings heavy down and right */
    u'┛', /* 251B box drawings heavy up and left */
    u'┓', /* 2513 box drawings heavy down and left */
    u'┗', /* 2517 box drawings heavy up and right */
    u'◢', /* 25E2 black lower right triangle */
    u'◣', /* 25E3 black lower left triangle */
    u'◥', /* 25E4 black upper right triangle */
    u'◤', /* 25E5 black upper left triangle */
    u'═', /* 2550 box drawings double horizontal */
    u'⎻', /* 23BB horizontal scan line 3 */
    u'⎼', /* 23BD horizontal scan line 9 */
};

/*───────────────────────────────────────────────────────────────────────────│─╗
│ derasterize § encoding                                                   ─╬─│┼
╚────────────────────────────────────────────────────────────────────────────│*/

/**
 * Returns binary logarithm of integer.
 * @dominion 𝑥≥1 ∧ 𝑥∊ℤ
 * @return [0,31)
 */
static unsigned bsr(unsigned x) {
#if -__STRICT_ANSI__ + !!(__GNUC__ + 0) && (__i386__ + __x86_64__ + 0)
    asm("bsr\t%1,%0" : "=r"(x) : "r"(x) : "cc");
#else
    static const unsigned char kDebruijn[32] = {
        0, 9,  1,  10, 13, 21, 2,  29, 11, 14, 16, 18, 22, 25, 3, 30,
        8, 12, 20, 28, 15, 17, 24, 7,  19, 27, 23, 6,  26, 5,  4, 31 };
    x |= x >> 1;
    x |= x >> 2;
    x |= x >> 4;
    x |= x >> 8;
    x |= x >> 16;
    x = kDebruijn[(x * 0x07c4acddu) >> 27];
#endif
    return x;
}

/**
 * Encodes Thompson-Pike variable length integer.
 *
 * @return word-encoded multibyte string
 * @note canonicalizes multibyte NUL and other numbers banned by the IETF
 * @note designed on a napkin in a New Jersey diner
 */
static unsigned long tpenc(wchar_t x) {
    if (0x00 <= x && x <= 0x7f) {
        return x;
    }
    else {
        static const struct ThomPike {
            unsigned char len, mark;
        } kThomPike[32 - 7] = {
            {1, 0xc0}, {1, 0xc0}, {1, 0xc0}, {1, 0xc0}, {2, 0xe0},
            {2, 0xe0}, {2, 0xe0}, {2, 0xe0}, {2, 0xe0}, {3, 0xf0},
            {3, 0xf0}, {3, 0xf0}, {3, 0xf0}, {3, 0xf0}, {4, 0xf8},
            {4, 0xf8}, {4, 0xf8}, {4, 0xf8}, {4, 0xf8}, {5, 0xfc},
            {5, 0xfc}, {5, 0xfc}, {5, 0xfc}, {5, 0xfc}, {5, 0xfc},
        };
        wchar_t wc;
        unsigned long ec;
        struct ThomPike op;
        ec = 0;
        wc = x;
        op = kThomPike[bsr(wc) - 7];
        do {
            ec |= 0x3f & wc;
            ec |= 0x80;
            ec <<= 010;
            wc >>= 006;
        } while (--op.len);
        return ec | wc | op.mark;
    }
}

/**
 * Formats Thompson-Pike variable length integer to array.
 *
 * @param p needs at least 8 bytes
 * @return p + number of bytes written, cf. mempcpy
 * @note no NUL-terminator is added
 */
static char* tptoa(char* p, wchar_t x) {
    unsigned long w;
    for (w = tpenc(x); w; w >>= 010) *p++ = w & 0xff;
    return p;
}

/**
 * Formats byte to array as decimal.
 *
 * @param p needs at least 4 bytes
 * @return p + number of bytes written, cf. mempcpy
 * @note call btoa(0,0) once at startup
 * @note no NUL-terminator is added
 */
static char* btoa(char* p, int c) {
    static char kBtoa[256][4];
    if (p) {
        memcpy(p, kBtoa[c & 0xff], 4);
        return p + (p[3] & 0xff);
    }
    else {
        for (c = 0; c < 256; ++c) {
            if (c < 10) {
                kBtoa[c][0] = '0' + c;
                kBtoa[c][3] = 1;
            }
            else if (c < 100) {
                kBtoa[c][0] = '0' + c / 10;
                kBtoa[c][1] = '0' + c % 10;
                kBtoa[c][3] = 2;
            }
            else {
                kBtoa[c][0] = '0' + c / 100;
                kBtoa[c][1] = '0' + c % 100 / 10;
                kBtoa[c][2] = '0' + c % 100 % 10;
                kBtoa[c][3] = 3;
            }
        }
        return 0;
    }
}

/*───────────────────────────────────────────────────────────────────────────│─╗
│ derasterize § colors                                                     ─╬─│┼
╚────────────────────────────────────────────────────────────────────────────│*/

/** Perform a sRGB gamma correction by power 2.4 approximation
 *
 * @param x Argument, in the range 0.09 to 1.
 * @return Value raised into power 2.4, approximate.
 */

static FLOAT pow24(FLOAT x) {
    FLOAT x2, x3, x4;
    x2 = x * x;
    x3 = x * x * x;
    x4 = x * x * x * x;
    return FLOAT_C(0.0985766365536824) + FLOAT_C(0.839474952656502) * x2 +
        FLOAT_C(0.363287814061725) * x3 -
        FLOAT_C(0.0125559718896615) /
        (FLOAT_C(0.12758338921578) + FLOAT_C(0.290283465468235) * x) -
        FLOAT_C(0.231757513261358) * x - FLOAT_C(0.0395365717969074) * x4;
}

/**
 * Approximately linearize the sRGB gamma value.
 *
 * @param s sRGB gamma value, in the range 0 to 1.
 * @return Linearized sRGB gamma value, approximated.
 */

static FLOAT frgb2linl(FLOAT x) {
    FLOAT r1, r2;
    r1 = x / FLOAT_C(12.92);
    r2 = pow24((x + FLOAT_C(0.055)) / (FLOAT_C(1.0) + FLOAT_C(0.055)));
    return x < FLOAT_C(0.04045) ? r1 : r2;
}

/**
 * Converts standard RGB to linear RGB.
 *
 * This makes subtraction look good by flattening out the bias curve
 * that PC display manufacturers like to use.
 */
static void rgb2lin(FLOAT f[CN * BN], const unsigned char u[CN * BN]) {
    unsigned i;
    for (i = 0; i < CN * BN; ++i) f[i] = u[i];
    for (i = 0; i < CN * BN; ++i) f[i] /= FLOAT_C(255.0);
    for (i = 0; i < CN * BN; ++i) f[i] = frgb2linl(f[i]);
}

/*───────────────────────────────────────────────────────────────────────────│─╗
│ derasterize § blocks                                                     ─╬─│┼
╚────────────────────────────────────────────────────────────────────────────│*/

struct Cell {
    char16_t rune;
    unsigned char bg[CN], fg[CN];
};

/**
 * Serializes ANSI background, foreground, and UNICODE glyph to wire.
 */
static char* celltoa(char* p, struct Cell cell, struct Cell last) {
    *p++ = 033;
    *p++ = '[';
    *p++ = '4';
    *p++ = '8';
    *p++ = ';';
    *p++ = '2';
    *p++ = ';';
    p = btoa(p, cell.bg[0]);
    *p++ = ';';
    p = btoa(p, cell.bg[1]);
    *p++ = ';';
    p = btoa(p, cell.bg[2]);
    *p++ = ';';
    *p++ = '3';
    *p++ = '8';
    *p++ = ';';
    *p++ = '2';
    *p++ = ';';
    p = btoa(p, cell.fg[0]);
    *p++ = ';';
    p = btoa(p, cell.fg[1]);
    *p++ = ';';
    p = btoa(p, cell.fg[2]);
    *p++ = 'm';
    p = tptoa(p, cell.rune);
    return p;
}

/**
 * Picks ≤2**MC unique (bg,fg) pairs from product of lb.
 */
static unsigned combinecolors(unsigned char bf[1u << MC][2],
    const unsigned char bl[CN * BN]) {
    uint64_t hv, ht[(1u << MC) * 2];
    unsigned i, j, n, b, f, h, hi, bu, fu;
    memset(ht, 0, sizeof(ht));
    for (n = b = 0; b < BN && n < (1u << MC); ++b) {
        bu = bl[2 * BN + b] << 020 | bl[1 * BN + b] << 010 | bl[0 * BN + b];
        hi = 0;
        hi = (((bu >> 000) & 0xff) + hi) * PHIPRIME;
        hi = (((bu >> 010) & 0xff) + hi) * PHIPRIME;
        hi = (((bu >> 020) & 0xff) + hi) * PHIPRIME;
        for (f = b + 1; f < BN && n < (1u << MC); ++f) {
            fu = bl[2 * BN + f] << 020 | bl[1 * BN + f] << 010 | bl[0 * BN + f];
            h = hi;
            h = (((fu >> 000) & 0xff) + h) * PHIPRIME;
            h = (((fu >> 010) & 0xff) + h) * PHIPRIME;
            h = (((fu >> 020) & 0xff) + h) * PHIPRIME;
            h = h & 0xffff;
            h = MAX(1, h);
            hv = 0;
            hv <<= 030;
            hv |= fu;
            hv <<= 030;
            hv |= bu;
            hv <<= 020;
            hv |= h;
            for (i = 0;; ++i) {
                j = (h + i * (i + 1) / 2) & (ARRAYLEN(ht) - 1);
                if (!ht[j]) {
                    ht[j] = hv;
                    bf[n][0] = b;
                    bf[n][1] = f;
                    n++;
                    break;
                }
                else if (ht[j] == hv) {
                    break;
                }
            }
        }
    }
    return n;
}

/**
 * Computes distance between synthetic block and actual.
 */
static FLOAT adjudicate(unsigned b, unsigned f, unsigned g,
    const FLOAT lb[CN * BN]) {
    unsigned i, k, gu;
    FLOAT p[BN], q[BN], fu, bu, r;
    memset(q, 0, sizeof(q));
    for (k = 0; k < CN; ++k) {
        gu = kGlyphs[g];
        bu = lb[k * BN + b];
        fu = lb[k * BN + f];
        for (i = 0; i < BN; ++i) p[i] = (gu & (1u << i)) ? fu : bu;
        for (i = 0; i < BN; ++i) p[i] -= lb[k * BN + i];
        // For a minimization problem, abs could do, but not faster in practice
        for (i = 0; i < BN; ++i) p[i] *= p[i];
        for (i = 0; i < BN; ++i) q[i] += p[i];
    }
    r = 0;
    // sqrt(x) is strictly increasing in x
    // so arg min sqrt(x) = arg min x
    // so we can go faster by simply commenting out
    // for (i = 0; i < BN; ++i) q[i] = SQRT(q[i]);
    for (i = 0; i < BN; ++i) r += q[i];
    return r;
}

/**
 * Converts tiny bitmap graphic into unicode glyph.
 */
static struct Cell derasterize(unsigned char block[CN * BN]) {
    struct Cell cell;
    FLOAT r, best, lb[CN * BN];
    unsigned i, n, b, f, g;
    unsigned char bf[1u << MC][2];
    rgb2lin(lb, block);
    n = combinecolors(bf, block);
    best = std::numeric_limits<float>::max();
    cell.rune = 0;
    for (i = 0; i < n; ++i) {
        b = bf[i][0];
        f = bf[i][1];
        for (g = 0; g < GN; ++g) {
            r = adjudicate(b, f, g, lb);
            if (r < best) {
                best = r;
                cell.rune = kRunes[g];
                cell.bg[0] = block[0 * BN + b];
                cell.bg[1] = block[1 * BN + b];
                cell.bg[2] = block[2 * BN + b];
                cell.fg[0] = block[0 * BN + f];
                cell.fg[1] = block[1 * BN + f];
                cell.fg[2] = block[2 * BN + f];
                if (!r) return cell;
            }
        }
    }
    return cell;
}

/*───────────────────────────────────────────────────────────────────────────│─╗
│ derasterize § graphics                                                   ─╬─│┼
╚────────────────────────────────────────────────────────────────────────────│*/

/**
 * Turns packed 8-bit RGB graphic into ANSI UNICODE text.
 */
static char* RenderImage(char* vt, const unsigned char* rgb, unsigned yn,
    unsigned xn) {
    char* v;
    struct Cell c1, c2;
    unsigned y, x, i, j, k, w;
    unsigned char block[CN * BN];
    const unsigned char* rows[YS];
    v = vt;
    c1.rune = 0;
    rgb -= (w = xn * XS * CN);
    for (y = 0; y < yn; ++y) {
        if (y) {
            while (v > vt && v[-1] == ' ') --v;
            *v++ = '\r';
            *v++ = '\n';
        }
        for (i = 0; i < YS; ++i) {
            rows[i] = (rgb += w) - XS * CN;
        }
        for (x = 0; x < xn; ++x) {
            for (i = 0; i < YS; ++i) {
                rows[i] += XS * CN;
                for (j = 0; j < XS; ++j) {
                    for (k = 0; k < CN; ++k) {
                        block[(k * YS + i) * XS + j] = rows[i][j * CN + k];
                    }
                }
            }
            c2 = derasterize(block);
            v = celltoa(v, c2, c1);
            c1 = c2;
        }
    }
    return v;
}

/*───────────────────────────────────────────────────────────────────────────│─╗
│ derasterize § systems                                                    ─╬─│┼
╚────────────────────────────────────────────────────────────────────────────│*/

// rgb should contains rgb24 data of size width x height
std::string PrintImage(void* rgb, unsigned yn, unsigned xn) {
    btoa(0, 0);

    char* v, * vt;
    vt = (char*)malloc(yn * (xn * (32 + (2 + (1 + 3) * 3) * 2 + 1 + 3)) * 1 + 5 + 1);
    v = RenderImage(vt, (unsigned char*)rgb, yn, xn);
    *v++ = '\r';
    *v++ = 033;
    *v++ = '[';
    *v++ = '0';
    *v++ = 'm';

    std::string result(vt, v - vt);
    //write(1, vt, v - vt);
    free(vt);

    return result;
}

/**
 * Determines dimensions of teletypewriter to default to full screen output
 
static void GetTermSize(unsigned* out_rows, unsigned* out_cols) {
    struct winsize ws;
    ws.ws_row = 24;
    ws.ws_col = 80;
    if (ioctl(STDIN_FILENO, TIOCGWINSZ, &ws) == -1) {
        ioctl(STDOUT_FILENO, TIOCGWINSZ, &ws);
    }
    *out_rows = ws.ws_row;
    *out_cols = ws.ws_col;
}
*/

/*
static void ReadAll(int fd, char* p, size_t n) {
    ssize_t rc;
    size_t got;
    do {
        ORDIE((rc = read(fd, p, n)) != -1);
        got = rc;
        if (!got && n) {
            exit(EXIT_FAILURE);
        }
        p += got;
        n -= got;
    } while (n);
}
*/

/*
static unsigned char* LoadImageOrDie(char* path, unsigned yn, unsigned xn) {
    void* rgb;
    size_t size;
    int pid, ws, rw[2];
    char dim[10 + 1 + 10 + 1 + 1];
    sprintf(dim, "%ux%u!", xn * XS, yn * YS);
    pipe(rw);
    if (!(pid = fork())) {
        close(rw[0]);
        dup2(rw[1], STDOUT_FILENO);
        execlp("convert", "convert", path, "-resize", dim, "-colorspace", "RGB",
            "-depth", "8", "rgb:-", NULL);
        _exit(EXIT_FAILURE);
    }
    close(rw[1]);
    ORDIE((rgb = valloc((size = yn * YS * xn * XS * CN))));
    ReadAll(rw[0], rgb, size);
    ORDIE(close(rw[0]) != -1);
    ORDIE(waitpid(pid, &ws, 0) != -1);
    ORDIE(WEXITSTATUS(ws) == 0);
    return rgb;
}
*/

/*
int main(int argc, char* argv[]) {
    int i, j;
    char* option, * filename;
    void* rgb;
    unsigned yd, xd;
    int y = 0, x = 0;

    btoa(0, 0); // FIXME: this is needed. But why?

    // Must provide at least one filename
    if (argc < 2) {
        printf(HELPTEXT);
        exit(255);
    }

    // Dirty option parsing without getopt
    for (i = 1; i < argc; ++i) {
        option = argv[i]; // option=-y12
        switch ((int)option[0]) {
        case '-': // unix style
        case '/': // dos style
            option++; // option=y12
            switch ((int)option[0]) {
            case 'x':
                x = atoi(++option);
                break;
            case 'y':
                y = atoi(++option);
                break;
            case 'h':
                printf(HELPTEXT);
                exit(1);
            default:
                printf("Unknown option %c\n\n", (int)option[0]);
            } // switch
        default:
            filename = option;
        } // switch
    } //for i

   // Use termize to default to full screen if no x and y are given
    GetTermSize(&yd, &xd);

    // if sizes are given, 2 cases:
    //  - positive values: use that as the target size
    //  - negative values: add, for ex to offset the command prompt size
    if (y <= 0) {
        y += yd;
    }
    if (x <= 0) {
        x += xd;
    }

    // FIXME: on the conversion stage should do 2Y because of halfblocks
    // printf( "filename >%s<\tx >%d<\ty >%d<\n\n", filename, x, y);
    rgb = LoadImageOrDie(filename, y, x);
    PrintImage(rgb, y, x);
    free(rgb);
    return 0;
}
*/