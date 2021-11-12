namespace RB.AuthorisationHold.ClientSample.Entities.Enums
{
	public enum HostType
	{
		Test,
		Prod
	}

	public enum ReservationType
	{
		Reserve,
		ReserveOrReplace,
		Confirm,
		Deposit,
		Withdrawal
	}

	public enum LogType
	{
		Info,
		Debug,
		Warn,
		Error,
		Fatal
	}

	public enum RandomStringType
	{
		Numeric,
		Alphabetic,
		SpacedAlphabetic,
		AlphaNum
	}
}