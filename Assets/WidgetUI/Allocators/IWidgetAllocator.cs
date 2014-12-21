
namespace WidgetUI
{
	public interface IWidgetAllocator<WidgetType> where WidgetType : IWidget
	{
		WidgetType Construct();
		void Destroy(WidgetType p_widget);
	}
}
