public enum Direction
{
	DOWN = 0,
	UP = 1,
	LEFT = 2,
	RIGHT = 3,
	UNSPECIFIED = -1
}

class DirectionHelper {
	public static Direction Opposite(Direction input)
	{
		return input ^ Direction.UP;
	}
}