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
using Models;

namespace WinApp.Views.HanhChinh
{
    /// <summary>
    /// Interaction logic for DonViSelector.xaml
    /// </summary>
    public partial class DonViSelector : UserControl
    {
        ViewDonVi _current;
        public string Caption
        {
            get => Title.Text;
            set => Title.Text = value;
        }
        public Models.HanhChinh Cap { get; set; }
        
        ViewDonVi _trucThuoc;
        public ViewDonVi TrucThuoc
        {
            get => _trucThuoc;
            set
            {
                _trucThuoc = value ?? new ViewDonVi();
                Caption = $"Các {Cap.Ten} thuộc {_trucThuoc.TenDayDu}";

                ShowItems();
            }
        }
        public ViewDonVi Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    OnSelected?.Invoke(_current);
                }
            }
        }
        public Action<ViewDonVi> OnSelected;
        
        public void ShowItems(Func<ViewDonVi, bool> condition)
        {
            Body.Children.Clear();

            _current = null;
            foreach (var item in DonVi.All.Where(condition).OrderBy(x => x.Ten))
            {
                var line = new DonViItemView {
                    Text = item.TenDayDu,
                };
                line.Click += (_s, _e) => {
                    Current = item;
                };

                Body.Children.Add(line);
                if (_current == null)
                {
                    Current = item;
                }
            }
        }
        public void ShowItems() => ShowItems(x => x.TrucThuocId == _trucThuoc.Id);
        protected virtual void RaiseAddNew()
        {
        }
        public DonViSelector()
        {
            InitializeComponent();

            AddNewButton.Click += (s, e) => { 
            
            };
        }
    }

    class DonViItemView : Vst.Controls.SideMenuButton
    {

    }
}
