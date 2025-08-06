using System.Windows;
using System.Windows.Controls;

namespace MZ.Resource.Managers
{
    /// <summary>
    /// 데이터 타입에 따라 DataTemplate을 반환
    /// </summary>
    public class EditorTemplateManager : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate Int32Template { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                string _ => StringTemplate,
                int _ => Int32Template,
                double _ => DoubleTemplate,
                bool _ => BoolTemplate,
                _ => base.SelectTemplate(item, container),
            };
        }
    }
}
