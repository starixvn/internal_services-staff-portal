namespace SSP.Models.Function
{
    using System;

    [Serializable]
    public class Function
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public int? ParentFunctionId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsActive { get; set; }
    }
}