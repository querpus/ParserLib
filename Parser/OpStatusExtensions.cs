namespace Parser;

public static class OpStatusExtensions
{
  extension(OpStatus status)
  {
    /// <summary>Checks a status for failure.</summary>
    /// <returns><see langword="true"/> if the status was a failure code, <see langword="false"/> otherwise.</returns>
    public bool IsFail => status.IsWithin(OpStatus.Fail, OpStatus.PastFail);
    /// <summary>Checks a status for success.</summary>
    public bool IsPass => status is OpStatus.EndCommand or OpStatus.Pass or OpStatus.Skipped or OpStatus.AtStart or OpStatus.FailOverride or OpStatus.ConditionPass;
    public bool IsEnd => status is OpStatus.EndCommand;
  }
}
