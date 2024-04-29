/*
	Poppler Wrapper: A C Binding for Poppler PDF Rendering Library
	Copyright (C) 2024 Lone Wolf Akela

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#pragma once

#ifdef POPPLERWRAPPER_EXPORTS
#define POPPLERWRAPPER_API __declspec(dllexport)
#else
#define POPPLERWRAPPER_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif
POPPLERWRAPPER_API char* poppler_extract_text_from_pdf_file(const char* filename, size_t filename_len = 0);
POPPLERWRAPPER_API void poppler_free_text(const char* text);
#ifdef __cplusplus
}
#endif
