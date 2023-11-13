#include <cstdint>
#include <cstring>
#include <vector>
#include <string>
#include <string_view>

#include <msclr/marshal_windows.h>

#include "Derasterize.h"

#include "lib.h"

namespace {
	// from https://blog.csdn.net/ZHANGKUN35268/article/details/100728856
	System::String^ to_system_string(std::string_view cpp_string, System::Text::Encoding^ enc)
	{
		array<unsigned char>^ c_array = gcnew array<unsigned char>((int)cpp_string.length());
		for (int i = 0; i < cpp_string.length(); i++)
		{
			c_array[i] = cpp_string[i];
		}
		System::String^ result = enc->GetString(c_array);
		return result;
	}
}

System::String^ Derasterize::ConvertImageToString(System::Drawing::Bitmap^ image)
{
	const int width = image->Width;
	const int height = image->Height;
	// make sure width is multiple of 4 and height is multiple of 8, or throw exception
	if (width % 4 != 0 || height % 8 != 0)
	{
		throw gcnew System::ArgumentException("Image width must be multiple of 4 and height must be multiple of 8");
	}

	const int x_chars = width / 4;
	const int y_chars = height / 8;

	// make sure image format is rgb24
	if (image->PixelFormat != System::Drawing::Imaging::PixelFormat::Format24bppRgb)
	{
		throw gcnew System::ArgumentException("Image format must be RGB24");
	}

	auto bitmap_data = image->LockBits(
		System::Drawing::Rectangle(0, 0, width, height),
		System::Drawing::Imaging::ImageLockMode::ReadOnly,
		image->PixelFormat);
	const int stride = System::Math::Abs(bitmap_data->Stride);

	auto raw_data = (uint8_t*)msclr::interop::marshal_as<HANDLE>(bitmap_data->Scan0);

	auto compact_data = std::vector<uint8_t>(width * height * 3);
	for (int line = 0; line < height; line++)
	{
		for (int x = 0; x < width; x++)
		{
			compact_data[line * width * 3 + x * 3 + 0] = raw_data[line * stride + x * 3 + 2];
			compact_data[line * width * 3 + x * 3 + 1] = raw_data[line * stride + x * 3 + 1];
			compact_data[line * width * 3 + x * 3 + 2] = raw_data[line * stride + x * 3 + 0];
		}
	}

	image->UnlockBits(bitmap_data);

	auto result = PrintImage(compact_data.data(), y_chars, x_chars);
	return to_system_string(result, System::Text::Encoding::UTF8);
}
