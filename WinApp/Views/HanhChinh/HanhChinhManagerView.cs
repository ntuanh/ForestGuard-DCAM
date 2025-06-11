using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vst.Controls;

namespace WinApp.Views.HanhChinh
{
    using TC = TableColumn;
    using TE = EditorInfo;

    class Huyen : HanhChinhView { }
    class Xa : HanhChinhView { }

    class HanhChinhView : BaseView<DataListViewLayout>
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Đơn vị hành chính";
            context.TableColumns = new TC[] {
                new TC { Name = "TenDayDu", Caption = "Tên đơn vị", Width = 250 },
                new TC { Name = "TrucThuoc", Caption = "Trực thuộc", Width = 120 },
            };
            context.Search = (o, s) =>
            {
                var e = (Models.ViewDonVi)o;
                return e.Ten.ToLower().Contains(s);
            };
        }
    }
    class Edit : Add
    {
        protected override void OnReady()
        {
            ShowDeleteAction("TenDayDu");
        }
    }
    class Add : EditView
    {
        protected override void RenderCore(ViewContext context)
        {
            base.RenderCore(context);
            context.Title = "Cập nhật đơn vị";
            context.Editors = new object[] {
                new TE { Name = "HanhChinhId", Caption = "Cấp hành chính", Layout = 4,
                    Type = "select", ValueName = "Id", DisplayName = "Ten",
                    Options = Provider.GetTable("HanhChinh").ToList<Models.HanhChinh>(null, null) },

                new TE { Name = "TrucThuocId", Caption = "Trực thuộc", Layout = 8,
                    Type = "select", ValueName = "Id", DisplayName = "TenDayDu", Options = Models.DonVi.All,
                },
                new TE { Name = "TenHanhChinh", Caption = "Tên hành chính", Layout = 4,
                    Type = "select", Options = Provider.GetTable("TenHanhChinh").ToList("Ten"), },

                new TE { Name = "Ten", Caption = "Tên đơn vị", Layout = 8 },
            };
        }

        MyEditor hanh_chinh;
        MyEditor truc_thuoc;
        void setTrucThuocOptions()
        {
            var id = hanh_chinh.Value;
            if (id == null)
            {
                truc_thuoc.Value = null;
                truc_thuoc.IsEnabled = false;
            }
            else
            {
                var opts = Models.DonVi.DanhSach((int)id - 1);

                truc_thuoc.SetOptions(opts);
                truc_thuoc.IsEnabled = opts.Count > 0;
            }
        }

        protected override void OnReady()
        {
            base.OnReady();
            hanh_chinh = FindEditor("HanhChinhId", null);
            truc_thuoc = FindEditor("TrucThuocId", null);

            setTrucThuocOptions();
            hanh_chinh.GetInput().LostFocus += (s, e) => setTrucThuocOptions();
        }
    }

    class Index : BaseView<GridLayout, IEnumerable<Models.ViewDonVi>>
    {
        class Section : DonViSelector
        {
            public Section(int cap, Section next) 
            {
                Cap = Models.DonVi.HanhChinh[cap];
                OnSelected = one => { 
                    if (next != null)
                    {
                        next.TrucThuoc = one;
                    }
                };

                AddNewButton.Click += (s, e) => {
                    App.RedirectToAction("add", new Models.ViewDonVi { 
                        HanhChinhId = Cap.Id, 
                        TrucThuocId = TrucThuoc.Id });
                };
            }
        }
        class SearchResult : DonViSelector { 
            public SearchResult()
            {
                Caption = "Kết quả tìm kiếm";
                Visibility = Visibility.Collapsed;

                AddNewButton.Click += (s, e) => App.RedirectToAction("add");
            }
        }
        Section trung_uong;
        Section quan_huyen;
        Section phuong_xa;
        SearchResult searchResult;
        PageHeader header;
        public Index()
        {
            phuong_xa = new Section(2, null);
            quan_huyen = new Section(1, phuong_xa);
            trung_uong = new Section(0, quan_huyen);
            header = new PageHeader();

            var main = new GridLayout().Split(2, 2);
            main.Add(trung_uong, 0, 0);
            main.Add(quan_huyen, 0, 1);
            main.Add(phuong_xa, 1, 1);

            searchResult = new SearchResult();
            searchResult.IsVisibleChanged += (s, e) => {
                main.Show((bool)e.OldValue); 
            };
            MainView.Split(2, 0);
            MainView.Rows[0].Height = new GridLength(50);

            MainView.Add(header, 0, 0);
            MainView.Add(main, 1, 0);
            MainView.Add(searchResult, 1, 0);

            trung_uong.SetRowSpan(2);

            header.PageTitle.Text = "Quản lý hành chính";
            header.SearchBox.Cleared += () => searchResult.Visibility = Visibility.Collapsed;
            header.SearchBox.Searching += s => {
                searchResult.ShowItems(x => x.Ten.ToLower().Contains(s));
                searchResult.Visibility = Visibility.Visible;
            };
        }

        protected override void OnReady()
        {
            trung_uong.TrucThuoc = new Models.ViewDonVi { TenHanhChinh = "Trung ương" };
        }
    }
}
