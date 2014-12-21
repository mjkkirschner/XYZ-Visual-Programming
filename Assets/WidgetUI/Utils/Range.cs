using System;

namespace WidgetUI
{
	public struct Range
	{
		public static readonly Range Invalid = new Range()
		{
			m_min = 0,
			m_count = -1
		};

		private int m_min, m_count;

		public Range(int p_min, int p_max)
		{
			m_min = p_min;
			m_count = p_max - m_min + 1;
		}

		public int Min
		{
			get
			{
				return m_min;
			}
			set
			{
				m_min = value;
			}
		}

		public int Max
		{
			get
			{
				return m_min + m_count - 1;
			}
			set
			{
				m_count = value - m_min + 1;
			}
		}

		public int ExclusiveMax
		{
			get
			{
				return m_min + m_count;
			}
		}

		public bool Contains(int p_value)
		{
			return p_value >= m_min && p_value < this.ExclusiveMax;
		}

		public void ForEach(Action<int> p_action)
		{
			int max = this.ExclusiveMax;

			for (int i = m_min; i < max; ++i)
			{
				p_action(i);
			}
		}
	}
}
