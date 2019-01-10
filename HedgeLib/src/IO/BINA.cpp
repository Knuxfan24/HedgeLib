#include "IO/BINA.h"
#include "IO/file.h"
#include <cstdint>
#include <memory>

namespace HedgeLib::IO::BINA
{
	void WriteOffsets(const HedgeLib::IO::File& file,
		const std::vector<std::uint32_t>& offsets) noexcept
	{
		// TODO: Big endian support
		std::uint32_t o, curOffset = 0;

		for (auto& offset : offsets)
		{
			o = ((offset - curOffset) >> 2);
			if (o > 0x3FFF)
			{
				o <<= 24;
				o |= ThirtyBit;
				file.Write(&o, sizeof(std::uint32_t), 1);
			}
			else if (o > 0x3F)
			{
				o <<= 8;
				o |= FourteenBit;
				file.Write(&o, sizeof(std::uint16_t), 1);
			}
			else
			{
				o |= SixBit;
				file.Write(&o, sizeof(std::uint8_t), 1);
			}

			curOffset = offset;
		}

		file.Pad();
	}
}