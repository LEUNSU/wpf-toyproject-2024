# wpf-toyproject-2024

**데이터포털 API 연동 프로그램**  

![시작화면](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf001.png)

## 배경
    지진이나 전쟁 등 재해 발생 시 효율적인 대피를 위한 대피장소 검색 

## 기능 
    - 부산광역시 지진 발생 시 옥외대피장소를 특정 '구'이름과 '동'이름을 선택하여 원하는 정보를 이용
    - 선택한 장소를 구글 지도와 연동하여 장소 및 길 찾기 가능
        
## 특징

- 공공데이터 API를 활용하여 json 데이터 추출

![JSON파일](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf010.png)

- Open API로 추출한 데이터를 DB에 저장 및 연동

![DB화면](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf009.png)

## 프로그램 실행

- 조회버튼을 누르고 저장을 누르면 API에서 불러온 데이터가 DB에 저장

![전체조회](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf002.png)

- API에서 '구'이름을 나타내는 데이터 값만 추출

![구선택](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf003.png)

- API에서 '구'이름을 나타내는 특정 데이터 값은 있었지만 '동'이름만 나타내는 데이터 값은 없었기때문에, 전체 주소에서 특정 '구'나 '군'에 속하는 '동'이름을 추출

![동선택](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf004.png)


    ``` 
    SELECT DISTINCT TRIM(REPLACE((RIGHT(b.Sub_adres, CHARINDEX(' ', b.Sub_adres) + 1)), '구 ', '')) AS Dong
                                                    FROM (  
                                                    SELECT LEFT(TRIM(REPLACE(Dtl_adres, '부산광역시', '')), PATINDEX('%[구군]%', Dtl_adres) - 1) AS Sub_adres
                                                        FROM AssemblyArea 
                                                        WHERE Dtl_adres LIKE '%' + @District + '%'
                                                    ) AS b
    ``` 

- 원하는 장소 선택하여 클릭하면, 구글 지도와 연동되어 장소 및 길 찾기 가능

![구글지도](https://raw.githubusercontent.com/LEUNSU/wpf-toyproject-2024/main/images/wpf008.png)

  
    ```
    public MapWindow(double Ycord, double Xcord) : this()
    {
        BrsLoc.Address = $"https://google.com/maps/place/{Ycord},{Xcord}";
    }
    ```

https://github.com/user-attachments/assets/11b13351-30f5-4bb6-9ad6-339e46e58843