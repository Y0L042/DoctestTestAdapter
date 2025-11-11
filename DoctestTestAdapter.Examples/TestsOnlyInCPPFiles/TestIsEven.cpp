#include <doctest.h>

inline bool IsEven(int number)
{
	return (number % 2 == 0);
}

TEST_CASE("[TestsOnlyInCPPFiles] - Is Even")
{
	CHECK(IsEven(2));
}
