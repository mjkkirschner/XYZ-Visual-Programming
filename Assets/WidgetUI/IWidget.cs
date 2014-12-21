
namespace WidgetUI
{
	public interface IWidget
	{
	}

	public interface IWidget<T> : IWidget
	{
		void Enable(T p_dataObject);
		void Disable();
	}
}
