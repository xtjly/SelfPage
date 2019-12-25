namespace SelfPage_Service.PageInfo
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// 来源类型 FromQuery|FromBody
        /// </summary>
        public FromEnumType FromType { get; set; } = FromEnumType.FromQuery;
        /// <summary>
        /// 数据类型 String|Int
        /// </summary>
        public ParameterDataType DataType { get; set; } = ParameterDataType.String;
        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; } = "";
        /// <summary>
        /// 参数默认值
        /// </summary>
        public object DefaultValue { get; set; } = null;
    }
}
