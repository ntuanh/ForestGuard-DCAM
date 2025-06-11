using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    partial class ViewDonVi
    {
        public string TenDayDu => $"{TenHanhChinh} {Ten}";
    }
    partial class DonVi
    {
        static List<ViewDonVi> _all;
        static public List<ViewDonVi> All
        {
            get
            {
                if (_all == null || _all.Count == 0)
                    _all = Provider.Select<ViewDonVi>();
                return _all;
            }
        }
        static public int? CapHanhChinhDangXuLy { get; set; }        
        static public List<ViewDonVi> DanhSach(int? cap)
        {
            if (cap == null)
                return All;

            var lst = new List<ViewDonVi>();
            lst.AddRange(All.Where(x => x.HanhChinhId == cap).OrderBy(x => x.Ten));
            return lst;
        }

        static HanhChinh[] _hanhChinh;
        static public HanhChinh[] HanhChinh
        {
            get
            {
                if (_hanhChinh == null)
                {
                    _hanhChinh = Provider.Select<HanhChinh>().ToArray();
                }
                return _hanhChinh;
            }
        }
    }
}
