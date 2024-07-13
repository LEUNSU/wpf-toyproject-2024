# wpf-toyproject-2024
WPF 개인프로젝트 리포지토리
**데이터포털 API 연동앱**  




https://github.com/LEUNSU/basic-wpf-2024/assets/158007401/c12f2301-bc7e-4950-b0cd-78ac9873d50e





- 기능 
    - 부산광역시 지진 발생 시 옥외대피장소를 특정 '구'이름과 '동'이름을 선택하여 원하는 정보를 볼 수 있음
    - 선택한 장소의 지도를 상세히 볼 수 있음
        
- 특징
    - 두 개의 콤보박스를 연동시킴으로써, '구'를 선택한 후 '구' 내의 '동'을 선택할 수 있음 (DB 사용)
    - API에서 제공하는 경도와 위도 값을 사용해 구글 지도와 연동하여, 선택한 주소 위치의 구글 지도가 나타남 

- 배운 점
    - API에서 '구'이름을 나타내는 특정 데이터 값은 있었지만 '동'이름만 나타내는 데이터 값은 없었기때문에, 전체 주소에서 특정 '구'나 '군'에 속하는 '동'이름을 추출함

    ``` 
    SELECT DISTINCT TRIM(REPLACE((RIGHT(b.Sub_adres, CHARINDEX(' ', b.Sub_adres) + 1)), '구 ', '')) AS Dong
                                                    FROM (  
                                                    SELECT LEFT(TRIM(REPLACE(Dtl_adres, '부산광역시', '')), PATINDEX('%[구군]%', Dtl_adres) - 1) AS Sub_adres
                                                        FROM AssemblyArea 
                                                        WHERE Dtl_adres LIKE '%' + @District + '%'
                                                    ) AS b
    ``` 

- 아쉬운 점
    - '구', '동' 선택 기능을 DB를 사용하지 않고 API에서 데이터를 바로 불러올 수 있도록 하는 방법이 있지만 아직 실력이 부족함 
    - API에서 데이터가 갱신되면 프로그램에서 오류가 생김
    - 범위를 부산광역시로 한정하였지만, API에서 제공하는 데이터는 전 지역이기때문에 전 지역을 범위로 하여 프로그램을 수정해 보고 싶음 
