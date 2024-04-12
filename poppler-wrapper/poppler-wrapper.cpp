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

#include "pch.h"

#include <poppler/cpp/poppler-document.h>
#include <poppler/cpp/poppler-page.h>

#include "poppler-wrapper.h"


char* poppler_extract_text_from_pdf_file(const char* filename, size_t filename_len)
{
	if (filename == nullptr)
	{
		return nullptr;
	}
	if (filename_len == 0)
	{
		filename_len = std::strlen(filename);
	}

	const std::string file_path(filename, filename_len);
	const std::unique_ptr<poppler::document> doc(poppler::document::load_from_file(file_path));

	if (!doc)
	{
		return nullptr;
	}

	const int pages = doc->pages();

	std::vector<char> all_text;

	for (int i = 0; i < pages; i++)
	{
		if (const std::unique_ptr<poppler::page> pg{ doc->create_page(i) })
		{
			auto page_text = pg->text().to_utf8();
			all_text.insert_range(all_text.end(), page_text);
			all_text.push_back('\n');
		}
	}

	const auto text_str = new char[all_text.size() + 1];
	std::ranges::copy(all_text, text_str);
	text_str[all_text.size()] = '\0';
	return text_str;
}

void poppler_free_text(const char* text)
{
	delete text;
}