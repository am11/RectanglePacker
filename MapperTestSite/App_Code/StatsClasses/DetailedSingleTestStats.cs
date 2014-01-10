using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MappingStats
/// </summary>
public class DetailedSingleTestStats : SingleTestStats
{
    public List<IterationStats> Iterations { get; set; }
}

