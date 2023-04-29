using System;

namespace HMMKayliesTweaks.Buttons
{
	class PageButtonTrigger : ButtonTrigger
	{
		public Action PageUpdate { get; set; }

		protected override void HandTriggered()
		{
			PageUpdate();
		}
	}
}
