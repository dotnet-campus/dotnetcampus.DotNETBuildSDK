using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace dotnetCampus.Comparison
{
    /// <summary>
    /// 配置 XML 比较
    /// </summary>
    public class XmlComparerSettings
    {
        /// <summary>
        /// 忽略的元素名，在此列表能找到的元素将会被忽略
        /// </summary>
        public IReadOnlyCollection<string>? IgnoreElementNameList { set; get; }

        /// <summary>
        /// 当前元素是否可以被忽略。此属性如果有设置，将会忽略 <see cref="IgnoreElementNameList"/> 属性
        /// </summary>
        public Func<XElement, bool>? CanElementIgnore { set; get; }
    }
}