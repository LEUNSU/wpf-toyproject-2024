using ControlzEx.Theming;
using System;

namespace AssemblyAreaList.Models
{
    public class AssemblyArea
    {
        public string Ctprvn_nm { get; set; } // 시도명
        public string Sgg_nm { get; set; } // 시군구명

        public string Vt_acmdfclty_nm { get; set; } // 시설명

        public string Dtl_adres { get; set; } // 상세 주소
        public string Rn_adres { get; set; } // 도로명 주소
        public string Fclty_ar { get; set; } // 시설 면적
        public string Vt_acmd_psbl_nmpr { get; set; } // 최대 수용 인원수
        public string Mngps_telno { get; set; } // 관리기관 전화번호
        public double Xcord { get; set; } // 경도
        public double Ycord { get; set; } // 위도


        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[AssemblyArea]
                                                                   ([Ctprvn_nm]
                                                                   ,[Sgg_nm]
                                                                   ,[Vt_acmdfclty_nm]
                                                                   ,[Dtl_adres]
                                                                   ,[Rn_adres]
                                                                   ,[Fclty_ar]
                                                                   ,[Vt_acmd_psbl_nmpr]
                                                                   ,[Mngps_telno]
                                                                   ,[Xcord]
                                                                   ,[Ycord])
                                                             VALUES
                                                                   (@Ctprvn_nm
                                                                   ,@Sgg_nm
                                                                   ,@Vt_acmdfclty_nm
                                                                   ,@Dtl_adres
                                                                   ,@Rn_adres
                                                                   ,@Fclty_ar
                                                                   ,@Vt_acmd_psbl_nmpr
                                                                   ,@Mngps_telno
                                                                   ,@Xcord
                                                                   ,@Ycord)";

        public static readonly string SELECT_QUERY_DISTRICT = @"SELECT  [Ctprvn_nm]
                                                                       ,[Sgg_nm]
                                                                       ,[Vt_acmdfclty_nm]
                                                                       ,[Dtl_adres]
                                                                       ,[Rn_adres] 
                                                                       ,[Fclty_ar]
                                                                       ,[Vt_acmd_psbl_nmpr]
                                                                       ,[Mngps_telno]
                                                                       ,[Xcord]
                                                                       ,[Ycord]
                                                                  FROM [dbo].[AssemblyArea]
                                                                 WHERE [Sgg_nm] = @Sgg_nm";

        public static readonly string GETDISTRICT_QUERY = @"SELECT [Sgg_nm] AS Save_District
                                                          FROM [dbo].[AssemblyArea]
                                                         GROUP BY [Sgg_nm]";

        public static string SELECT_QUERY_NEIGHBORHOOD = @"SELECT [Ctprvn_nm]
                                                                 ,[Sgg_nm]
                                                                 ,[Vt_acmdfclty_nm]
                                                                 ,[Dtl_adres]
                                                                 ,[Rn_adres] 
                                                                 ,[Fclty_ar]
                                                                 ,[Vt_acmd_psbl_nmpr]
                                                                 ,[Mngps_telno]
                                                                 ,[Xcord]
                                                                 ,[Ycord]
                                                            FROM [dbo].[AssemblyArea]
                                                           WHERE [Sgg_nm] = @Sgg_nm AND Dtl_adres LIKE '%' + @Dong + '%'";

        public static string SELECT_QUERY_DONG = @"SELECT DISTINCT TRIM(REPLACE((RIGHT(b.Sub_adres, CHARINDEX(' ', b.Sub_adres) + 1)), '구 ', '')) AS Dong
	                                                FROM (  
	                                                SELECT LEFT(TRIM(REPLACE(Dtl_adres, '부산광역시', '')), PATINDEX('%[구군]%', Dtl_adres) - 1) AS Sub_adres
		                                                FROM AssemblyArea 
		                                                WHERE Dtl_adres LIKE '%' + @District + '%'
	                                                ) AS b";

    }
}