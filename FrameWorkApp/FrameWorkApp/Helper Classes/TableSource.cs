
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace FrameWorkApp
{
	//Source: http://docs.xamarin.com/guides/ios/user_interface/tables/part_2_-_populating_a_table_with_data
	public class TableSource : UITableViewSource {
		string[] tableItems;
		string cellIdentifier = "TableCell";
		public TableSource (string[] items)
		{
			tableItems = items;
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Length;
		}
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			cell.TextLabel.Text = tableItems[indexPath.Row];
			return cell;
		}
	}
}

