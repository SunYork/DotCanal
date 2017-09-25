using System;

namespace DotCanal.Driver.MySqlClient
{
    [Flags]
    public enum ServerStatusFlags
    {
        InTransaction = 1, // Transaction has started
        AutoCommitMode = 2, // Server in auto_commit mode 
        MoreResults = 4, // More results on server
        AnotherQuery = 8, // Multi query - next query exists
        BadIndex = 16,
        NoIndex = 32,
        CursorExists = 64,
        LastRowSent = 128,
        OutputParameters = 4096
    }
}
