using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using DocumentFormat.OpenXml.ExtendedProperties;
using Markdig;
using Markdig.Extensions.Mathematics;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Wpf;
using Markdig.Syntax;
using Markdig.Wpf;
using WpfMath.Controls;
using Application = System.Windows.Application;

namespace ChatGptApiClientV2;

public static class MarkdownPipelineBuilderExtension
{
    public static MarkdownPipelineBuilder UseMathBlock(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready<WpfMarkdownMathExtension>();
        return pipeline;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountAndSkipChar(this StringSlice slice, char matchChar)
    {
        var text = slice.Text;
        var end = slice.End;
        var current = slice.Start;

        while (current <= end && (uint)current < (uint)text.Length && text[current] == matchChar)
        {
            current++;
        }

        var count = current - slice.Start;
        slice.Start = current;
        return count;
    }
}

public class WpfMarkdownMathExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<MarkdownMathInlineParser>())
        {
            pipeline.InlineParsers.Insert(0, new MarkdownMathInlineParser());
        }
        if (!pipeline.BlockParsers.Contains<MarkdownMathBlockParser>())
        {
            pipeline.BlockParsers.Insert(0, new MarkdownMathBlockParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not WpfRenderer wpfRenderer)
        {
            return;
        }

        if (!wpfRenderer.ObjectRenderers.Contains<WpfMarkdownMathInlineRenderer>())
        {
            wpfRenderer.ObjectRenderers.Insert(0, new WpfMarkdownMathInlineRenderer());
        }
        if (!wpfRenderer.ObjectRenderers.Contains<WpfMarkdownMathBlockRenderer>())
        {
            wpfRenderer.ObjectRenderers.Insert(0, new WpfMarkdownMathBlockRenderer());
        }
    }
}

public class WpfMarkdownMathBlockRenderer : WpfObjectRenderer<MathBlock>
{
    protected override void Write(WpfRenderer renderer, MathBlock obj)
    {
        // drop first and last lines
        var lines = obj.Lines.Lines[1..^2];
        var formula = new FormulaControl
        {
            Style = Application.Current.FindResource("CustomFormulaStyle") as Style,
            Formula = string.Join('\n', lines)
        };
        var block = new BlockUIContainer(formula);
        renderer.WriteBlock(block);
    }
}

public class WpfMarkdownMathInlineRenderer : WpfObjectRenderer<MathInline>
{
    protected override void Write(WpfRenderer renderer, MathInline obj)
    {
        var formula = new FormulaControl
        {
            Style = Application.Current.FindResource("CustomFormulaStyle") as Style,
            Formula = obj.Content.ToString(),
        };
        var inline = new InlineUIContainer(formula)
        {
            BaselineAlignment = BaselineAlignment.Center
        };
        renderer.WriteInline(inline);
    }
}

public class MarkdownMathBlockParser : BlockParser
{
    public MarkdownMathBlockParser()
    {
        OpeningCharacters = ['$', '\\'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        var line = processor.Line;
        var matchChar = line.CurrentChar;
        var nextChar = line.PeekChar();
        if (!(matchChar == '$' && nextChar == '$') && !(matchChar == '\\' && nextChar == '['))
        {
            return BlockState.None;
        }

        var block = new MathBlock(this)
        {
            Column = processor.Column,
            FencedChar = matchChar,
            Span = {
                Start = processor.Start,
                End = line.Start
            }
        };

        processor.NewBlocks.Push(block);
        return BlockState.Continue;
    }

    public override BlockState TryContinue(BlockProcessor processor, Markdig.Syntax.Block block)
    {
        var mathBlock = (MathBlock)block;
        var line = processor.Line;
        var sourcePosition = processor.Start;

        var matchChar = mathBlock.FencedChar;
        var currentChar = line.CurrentChar;
        var nextChar = line.PeekChar();

        if (matchChar != currentChar)
        {
            return BlockState.Continue;
        }
        if (!(currentChar == '$' && nextChar == '$') && !(currentChar == '\\' && nextChar == ']'))
        {
            return BlockState.Continue;
        }

        mathBlock.UpdateSpanEnd(line.End);
        return BlockState.Break;
    }
}

public class MarkdownMathInlineParser : InlineParser
{
    public MarkdownMathInlineParser()
    {
        OpeningCharacters = ['$', '\\'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        return slice.CurrentChar switch
        {
            '$' => MatchDollars(processor, ref slice),
            '\\' => MatchBrackets(processor, ref slice),
            _ => false
        };
    }

    private static bool MatchDollars(InlineProcessor processor, ref StringSlice slice)
    {
        var match = slice.CurrentChar;
        var pc = slice.PeekCharExtra(-1);
        if (pc == match)
        {
            return false;
        }

        var startPosition = slice.Start;

        // Match the opened $ or $$
        char c;
        var openDollars = 0; // we have at least a $
        do
        {
            openDollars += 1;
            c = slice.NextChar();
        } while (c == match);

        pc.CheckUnicodeCategory(out var openPrevIsWhiteSpace, out var openPrevIsPunctuation);
        c.CheckUnicodeCategory(out var openNextIsWhiteSpace, out _);

        // Check that opening $/$$ is correct, using the different heuristics than for emphasis delimiters
        // If a $/$$ is not preceded by a whitespace or punctuation, this is a not a math block
        if ((!openPrevIsWhiteSpace && !openPrevIsPunctuation))
        {
            return false;
        }

        var closeDollars = 0;

        // Eat any leading spaces
        while (c.IsSpaceOrTab())
        {
            c = slice.NextChar();
        }

        var start = slice.Start;
        var end = 0;

        pc = match;
        var lastWhiteSpace = -1;
        while (c != '\0')
        {
            // Don't allow newline in an inline math expression
            if (c is '\r' or '\n')
            {
                return false;
            }

            // Don't process sticks if we have a '\' as a previous char
            if (pc != '\\')
            {
                // Record continuous whitespaces at the end
                if (c.IsSpaceOrTab())
                {
                    if (lastWhiteSpace < 0)
                    {
                        lastWhiteSpace = slice.Start;
                    }
                }
                else
                {
                    var hasClosingDollars = c == match;
                    if (hasClosingDollars)
                    {
                        closeDollars += slice.CountAndSkipChar(match);
                        c = slice.NextChar();
                    }

                    if (closeDollars >= openDollars)
                    {
                        break;
                    }

                    lastWhiteSpace = -1;
                    if (hasClosingDollars)
                    {
                        pc = match;
                        continue;
                    }
                }
            }

            if (closeDollars > 0)
            {
                closeDollars = 0;
            }
            else
            {
                pc = c;
                c = slice.NextChar();
            }
        }

        if (closeDollars < openDollars)
        {
            return false;
        }

        pc.CheckUnicodeCategory(out var closePrevIsWhiteSpace, out _);
        c.CheckUnicodeCategory(out var closeNextIsWhiteSpace, out var closeNextIsPunctuation);

        // A closing $/$$ should be followed by at least a punctuation or a whitespace
        // and if the character after an opening $/$$ was a whitespace, it should be
        // a whitespace as well for the character preceding the closing of $/$$
        if ((!closeNextIsPunctuation && !closeNextIsWhiteSpace) || (openNextIsWhiteSpace != closePrevIsWhiteSpace))
        {
            return false;
        }

        if (closePrevIsWhiteSpace && lastWhiteSpace > 0)
        {
            end = lastWhiteSpace + openDollars - 1;
        }
        else
        {
            end = slice.Start - 1;
        }

        // Create a new MathInline
        var inline = new MathInline
        {
            Span = new SourceSpan(
                processor.GetSourcePosition(startPosition, out var line, out var column), 
                processor.GetSourcePosition(slice.Start - 1)),
            Line = line,
            Column = column,
            Delimiter = match,
            DelimiterCount = openDollars,
            Content = slice
        };
        inline.Content.Start = start;
        // We subtract the end to the number of opening $ to keep inside the block the additional $s
        inline.Content.End = end - openDollars;

        processor.Inline = inline;

        return true;
    }

    private static bool MatchBrackets(InlineProcessor processor, ref StringSlice slice)
    {
        var pc = slice.PeekCharExtra(-1);
        pc.CheckUnicodeCategory(out var openPrevIsWhiteSpace, out var openPrevIsPunctuation);

        var startPosition = slice.Start;

        if (slice.NextChar() != '(')
        {
            return false;
        }

        var c = slice.NextChar();
        c.CheckUnicodeCategory(out var openNextIsWhiteSpace, out _);

        // Check that opening `\(` is correct, using the different heuristics than for emphasis delimiters
        // If a `\(` is not preceded by a whitespace or punctuation, this is a not a math block
        if ((!openPrevIsWhiteSpace && !openPrevIsPunctuation))
        {
            return false;
        }

        // Eat any leading spaces
        while (c.IsSpaceOrTab())
        {
            c = slice.NextChar();
        }

        var start = slice.Start;
        var end = 0;

        pc = slice.PeekCharExtra(-1);
        var lastWhiteSpace = -1;
        var foundEnd = false;
        while (c != '\0')
        {
            // Don't allow newline in an inline math expression
            if (c is '\r' or '\n')
            {
                return false;
            }

            // Don't process sticks if we have a '\' as a previous char
            if (pc != '\\')
            {
                // Record continuous whitespaces at the end
                if (c.IsSpaceOrTab())
                {
                    if (lastWhiteSpace < 0)
                    {
                        lastWhiteSpace = slice.Start;
                    }
                }
                else
                {
                    if (c == '\\' && slice.PeekChar() == ')')
                    {
                        slice.NextChar();
                        slice.NextChar();
                        foundEnd = true;
                        break;
                    }

                    lastWhiteSpace = -1;
                }
            }

            pc = c;
            c = slice.NextChar();
        }

        if (!foundEnd)
        {
            return false;
        }

        pc.CheckUnicodeCategory(out var closePrevIsWhiteSpace, out _);
        c.CheckUnicodeCategory(out var closeNextIsWhiteSpace, out var closeNextIsPunctuation);

        // A closing $/$$ should be followed by at least a punctuation or a whitespace
        // and if the character after an opening $/$$ was a whitespace, it should be
        // a whitespace as well for the character preceding the closing of $/$$
        if ((!closeNextIsPunctuation && !closeNextIsWhiteSpace) || (openNextIsWhiteSpace != closePrevIsWhiteSpace))
        {
            return false;
        }

        if (closePrevIsWhiteSpace && lastWhiteSpace > 0)
        {
            end = lastWhiteSpace + 1;
        }
        else
        {
            end = slice.Start - 1;
        }

        // Create a new MathInline
        var inline = new MathInline
        {
            Span = new SourceSpan(
                processor.GetSourcePosition(startPosition, out var line, out var column),
                processor.GetSourcePosition(slice.Start - 1)),
            Line = line,
            Column = column,
            Delimiter = '\\',
            DelimiterCount = 2,
            Content = slice
        };
        inline.Content.Start = start;
        // We subtract the end of `\)`
        inline.Content.End = end - 2;

        processor.Inline = inline;

        return true;
    }
}