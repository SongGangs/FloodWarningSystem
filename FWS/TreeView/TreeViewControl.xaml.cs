using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FWS.TreeView
{
    /// <summary>
    /// TreeViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class TreeViewControl : UserControl
    {
        #region 私有变量属性

        /// <summary>
        /// 控件数据
        /// </summary>
        private IList<TreeView.TreeModel> _itemsSourceData;

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public TreeViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件数据
        /// </summary>
        public IList<TreeView.TreeModel> ItemsSourceData
        {
            get { return _itemsSourceData; }
            set
            {
                _itemsSourceData = value;
                Tree.ItemsSource = _itemsSourceData;
            }
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id, IList<TreeView.TreeModel> treeList)
        {
            foreach (var tree in treeList)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id)
        {
            foreach (var tree in ItemsSourceData)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public IList<TreeView.TreeModel> CheckedItemsIgnoreRelation()
        {

            return GetCheckedItemsIgnoreRelation(_itemsSourceData);
        }

        /// <summary>
        /// 私有方法，忽略层次关系的情况下，获取选中项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<TreeView.TreeModel> GetCheckedItemsIgnoreRelation(IList<TreeView.TreeModel> list)
        {
            IList<TreeView.TreeModel> treeList = new List<TreeView.TreeModel>();
            foreach (var tree in list)
            {
                if (tree.IsChecked)
                {
                    treeList.Add(tree);
                }
            }
            return treeList;
        }

        /// <summary>
        /// 缩放至图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuZoomIn_Click(object sender, RoutedEventArgs e)
        {
            
        }


        /// <summary>
        /// 全部选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (TreeView.TreeModel tree in Tree.ItemsSource)
                {
                    tree.IsChecked = true;
                }
            }
            catch (Exception)
            {
            }
            
        }

        /// <summary>
        /// 全部取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (TreeView.TreeModel tree in Tree.ItemsSource)
                {
                    tree.IsChecked = false;
                }
            }
            catch (Exception)
            {
            }
           
        }

        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof (T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
