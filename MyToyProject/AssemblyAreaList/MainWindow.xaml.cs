using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using AssemblyAreaList.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace AssemblyAreaList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitComboDistrictFromDB();
            //InitComboNeighborhoodFromDB();
        }

        private void InitComboDistrictFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.AssemblyArea.GETDISTRICT_QUERY, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dSet = new DataSet();
                adapter.Fill(dSet);
                List<string> saveDistricts = new List<string>();

                foreach (DataRow row in dSet.Tables[0].Rows)
                {
                    saveDistricts.Add(Convert.ToString(row["Save_District"]));
                }

                CboDistrict.ItemsSource = saveDistricts;
            }
        }
        //private void InitComboNeighborhoodFromDB()
        //{
        //    using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand(Models.AssemblyArea.GETDONG_QUERY, conn);
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataSet dSet = new DataSet();
        //        adapter.Fill(dSet);
        //        List<string> saveNeighborhoods = new List<string>();

        //        foreach (DataRow row in dSet.Tables[0].Rows)
        //        {
        //            saveNeighborhoods.Add(Convert.ToString(row["Save_Neighborhood"]));
        //        }

        //        CboNeighborhood.ItemsSource = saveNeighborhoods;
        //    }
        //}

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://apis.data.go.kr/1741000/EmergencyAssemblyArea_Earthquake5/getArea4List2?ServiceKey=2vtB4IK7A0Zdd3%2B0mcat%2FfF%2BiLxEh3BGGnmVdWl3p8N3e6k%2FklfFCL7q2rG%2FXW1FAwGS2KNUK6iAyZSZRs%2B31w%3D%3D&pageNo=1&numOfRows=200&type=JSON&ctprvn_nm=%EB%B6%80%EC%82%B0%EA%B4%91%EC%97%AD%EC%8B%9C";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(url);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                // await this.ShowMessageAsync("결과", result);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            // 데이터 출력
            var data1 = jsonResult["EarthquakeOutdoorsShelter2"][0];
            var data5 = Convert.ToInt32(data1["head"][0]["totalCount"]);

            if (data5 > 0)
            {
                var data6 = jsonResult["EarthquakeOutdoorsShelter2"][1]["row"];
                var jsonArray = data6 as JArray; // json자체에서 []안에 들어간 배열데이터만 Array 변환가능

                var assemblyArea = new List<AssemblyArea>();
                foreach (var item in jsonArray)
                {
                    assemblyArea.Add(new AssemblyArea()
                    {
                        Ctprvn_nm = Convert.ToString(item["ctprvn_nm"]),
                        Sgg_nm = Convert.ToString(item["sgg_nm"]),
                        Vt_acmdfclty_nm = Convert.ToString(item["vt_acmdfclty_nm"]),
                        Dtl_adres = Convert.ToString(item["dtl_adres"]),
                        Rn_adres = Convert.ToString(item["rn_adres"]),
                        Fclty_ar = Convert.ToString(item["fclty_ar"]),
                        Vt_acmd_psbl_nmpr = Convert.ToString(item["vt_acmd_psbl_nmpr"]),
                        Mngps_telno = Convert.ToString(item["mngps_telno"]),
                        Xcord = Convert.ToDouble(item["xcord"]),
                        Ycord = Convert.ToDouble(item["ycord"])
                    });
                }

                this.DataContext = assemblyArea;
                StsResult.Content = $"OpenAPI {assemblyArea.Count}건 조회완료!";
            }
        }
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("저장오류", "실시간 조회후 저장하십시오.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (AssemblyArea item in GrdResult.Items)
                    {
                        SqlCommand cmd = new SqlCommand(Models.AssemblyArea.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Ctprvn_nm", item.Ctprvn_nm);
                        cmd.Parameters.AddWithValue("@Sgg_nm", item.Sgg_nm);
                        cmd.Parameters.AddWithValue("@Vt_acmdfclty_nm", item.Vt_acmdfclty_nm);
                        cmd.Parameters.AddWithValue("@Dtl_adres", item.Dtl_adres);
                        cmd.Parameters.AddWithValue("@Rn_adres", item.Rn_adres);
                        cmd.Parameters.AddWithValue("@Fclty_ar", item.Fclty_ar);
                        cmd.Parameters.AddWithValue("@Vt_acmd_psbl_nmpr", item.Vt_acmd_psbl_nmpr);
                        cmd.Parameters.AddWithValue("@Mngps_telno", item.Mngps_telno);
                        cmd.Parameters.AddWithValue("@Xcord", item.Xcord);
                        cmd.Parameters.AddWithValue("@Ycord", item.Ycord);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "DB저장성공!");
                        StsResult.Content = $"DB저장 {insRes}건 성공!";
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류 {ex.Message}");
            }

            InitComboDistrictFromDB();
            //InitComboNeighborhoodFromDB();
        }

        private void CboDistrict_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CboDistrict.SelectedValue != null)
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(Models.AssemblyArea.SELECT_QUERY_DISTRICT, conn);
                    cmd.Parameters.AddWithValue("@Sgg_nm", CboDistrict.SelectedValue.ToString());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dSet = new DataSet();
                    adapter.Fill(dSet, "AssemblyArea");
                    var assemblyArea = new List<AssemblyArea>();

                    foreach (DataRow row in dSet.Tables["AssemblyArea"].Rows)
                    {
                        assemblyArea.Add(new AssemblyArea
                        {
                            Ctprvn_nm = Convert.ToString(row["ctprvn_nm"]),
                            Sgg_nm = Convert.ToString(row["sgg_nm"]),
                            Vt_acmdfclty_nm = Convert.ToString(row["vt_acmdfclty_nm"]),
                            Dtl_adres = Convert.ToString(row["dtl_adres"]),
                            Rn_adres = Convert.ToString(row["rn_adres"]),
                            Fclty_ar = Convert.ToString(row["fclty_ar"]),
                            Vt_acmd_psbl_nmpr = Convert.ToString(row["vt_acmd_psbl_nmpr"]),
                            Mngps_telno = Convert.ToString(row["mngps_telno"]),
                            Xcord = Convert.ToDouble(row["xcord"]),
                            Ycord = Convert.ToDouble(row["ycord"])
                        });
                    }

                    this.DataContext = assemblyArea;
                    StsResult.Content = $"DB {assemblyArea.Count}건 조회완료";
                }

                // 선택된 값으로 동선택 콤보박스 값을 DB에서 읽어와서 출력
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(Models.AssemblyArea.SELECT_QUERY_DONG, conn);
                    cmd.Parameters.AddWithValue("@District", CboDistrict.SelectedValue.ToString());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dSet = new DataSet();
                    adapter.Fill(dSet);
                    List<string> saveDong = new List<string>();

                    foreach (DataRow row in dSet.Tables[0].Rows)
                    {
                        saveDong.Add(Convert.ToString(row["Dong"]));
                    }

                    CboNeighborhood.ItemsSource = saveDong;
                }
            }
            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회클리어";
            }

        }

        private void GrdResult_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as AssemblyArea;

            var mapWindow = new MapWindow(curItem.Ycord, curItem.Xcord);
            mapWindow.Owner = this;
            mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();
        }

        private void CboNeighborhood_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CboDistrict.SelectedValue != null && CboNeighborhood.SelectedValue != null)
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    // 선택된 구와 동에 맞는 데이터를 조회
                    SqlCommand cmd = new SqlCommand(Models.AssemblyArea.SELECT_QUERY_NEIGHBORHOOD, conn);
                    cmd.Parameters.AddWithValue("@Sgg_nm", CboDistrict.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@Dong", CboNeighborhood.SelectedValue.ToString());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dSet = new DataSet();
                    adapter.Fill(dSet, "AssemblyArea");
                    var assemblyArea = new List<AssemblyArea>();

                    foreach (DataRow row in dSet.Tables["AssemblyArea"].Rows)
                    {
                        assemblyArea.Add(new AssemblyArea
                        {
                            Ctprvn_nm = Convert.ToString(row["ctprvn_nm"]),
                            Sgg_nm = Convert.ToString(row["sgg_nm"]),
                            Vt_acmdfclty_nm = Convert.ToString(row["vt_acmdfclty_nm"]),
                            Dtl_adres = Convert.ToString(row["dtl_adres"]),
                            Rn_adres = Convert.ToString(row["rn_adres"]),
                            Fclty_ar = Convert.ToString(row["fclty_ar"]),
                            Vt_acmd_psbl_nmpr = Convert.ToString(row["vt_acmd_psbl_nmpr"]),
                            Mngps_telno = Convert.ToString(row["mngps_telno"]),
                            Xcord = Convert.ToDouble(row["xcord"]),
                            Ycord = Convert.ToDouble(row["ycord"])
                        });
                    }

                    // 조회된 데이터를 DataContext에 바인딩하여 UI에 표시
                    this.DataContext = assemblyArea;
                    StsResult.Content = $"DB {assemblyArea.Count}건 조회완료";
                }
            }
        }
    }
}