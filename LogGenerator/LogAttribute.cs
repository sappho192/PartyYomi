using System;

namespace LogGenerator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TraceLogger : Attribute
    {
    }
}
