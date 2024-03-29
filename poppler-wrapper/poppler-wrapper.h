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
