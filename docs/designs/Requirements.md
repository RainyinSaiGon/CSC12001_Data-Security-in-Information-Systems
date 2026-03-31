# Trường Đại học Khoa học Tự nhiên
## Khoa Công nghệ Thông tin

**Năm học:** 2025-2026  
**Đồ án môn học:** CSC12001 - An toàn và bảo mật dữ liệu trong HTTT

**Giảng viên:**
- TS. Phạm Thị Bạch Huệ
- ThS. Lương Vĩ Minh
- ThS. Tiết Gia Hồng

## Nội dung

- [Phân hệ 1: Ứng dụng quản trị CSDL Oracle](#phân-hệ-1-ứng-dụng-quản-trị-csdl-oracle)
- [Phân hệ 2: Ứng dụng quản lý dữ liệu y tế](#phân-hệ-2-ứng-dụng-quản-lý-dữ-liệu-y-tế)
- [Yêu cầu 1: Giải pháp cấp quyền truy cập và cài đặt giao diện](#yêu-cầu-1-giải-pháp-cấp-quyền-truy-cập-và-cài-đặt-giao-diện)
- [Yêu cầu 2: Cơ chế phát tán thông báo dùng OLS và cài đặt giao diện](#yêu-cầu-2-cơ-chế-phát-tán-thông-báo-dùng-ols-và-cài-đặt-giao-diện)
- [Yêu cầu 3: Vận dụng cơ chế kiểm toán](#yêu-cầu-3-vận-dụng-cơ-chế-kiểm-toán)
- [Yêu cầu 4: Sao lưu và phục hồi dữ liệu](#yêu-cầu-4-sao-lưu-và-phục-hồi-dữ-liệu)
- [Một số quy định](#một-số-quy-định)

## Phân hệ 1: Ứng dụng quản trị CSDL Oracle

Dành cho người quản trị cơ sở dữ liệu.

Sinh viên hãy xây dựng ứng dụng WinForm cho phép người dùng có quyền quản trị trên Oracle DB Server thực hiện được các thao tác sau:

1. Cho phép tạo mới, xóa, sửa (hiệu chỉnh) user hoặc role.
2. Xem danh sách tài khoản người dùng và role trong hệ thống Oracle DB Server.
3. Cho phép thực hiện việc cấp quyền:
   - cấp quyền cho user, cấp quyền cho role, cấp role cho user
   - quá trình cấp quyền có tùy chọn là có cho phép người được cấp quyền có thể cấp quyền đó cho user hoặc role khác hay không, tức có chỉ định `WITH GRANT OPTION` hay không
   - thực hiện cấp quyền trên một số loại đối tượng của CSDL như: `table`, `view`, `stored procedure`, `function`; quyền `select`, `update` phải cho phép phân quyền tính đến mức cột; quyền `insert`, `delete` thì không. Lưu ý, quyền trên các loại đối tượng khác nhau có thể khác nhau
4. Cho phép thu hồi quyền từ user hoặc role.
5. Xem thông tin về quyền của mỗi user hoặc role trên các đối tượng dữ liệu.

## Phân hệ 2: Ứng dụng quản lý dữ liệu y tế

Một bệnh viện X quản lý việc khám chữa bệnh thông qua một hệ thống thông tin quản lý `S`.

### Mô hình dữ liệu

#### BỆNHNHÂN

`BỆNHNHÂN(MÃBN, TÊNBN, PHÁI, NGÀYSINH, CCCD, SỐNHÀ, TÊNĐƯỜNG, QUẬNHUYỆN, TỈNHTP, TIỀNSỬBỆNH, TIỀNSỬBỆNHGD, DỊỨNGTHUỐC)`

Mỗi bệnh nhân được bệnh viện cấp mã duy nhất (`MÃBN`), có tên (`TÊNBN`), phái (`PHÁI`), ngày sinh (`NGÀYSINH`), căn cước công dân (`CCCD`), địa chỉ (`SỐNHÀ`, `TÊNĐƯỜNG`, `QUẬNHUYỆN`, `TỈNHTP`), và tiền sử bệnh của bệnh nhân (`TIỀNSỬBỆNH`) và gia đình (`TIỀNSỬBỆNHGD`), cũng như tình trạng dị ứng thuốc nếu có (`DỊỨNGTHUỐC`).

#### NHÂNVIÊN

`NHÂNVIÊN(MÃNV, HỌTÊN, PHÁI, NGÀYSINH, CMND, QUÊQUÁN, SỐĐT, VAITRÒ, CHUYÊNKHOA)`

Quan hệ `NHÂNVIÊN` chứa dữ liệu về các nhân viên trong bệnh viện. Mỗi nhân viên có mã (`MÃNV`), họ tên (`HỌTÊN`), phái (`PHÁI`), ngày sinh (`NGÀYSINH`), số chứng minh nhân dân (`CMND`), quê quán (`QUÊQUÁN`), số điện thoại (`SỐĐT`), thuộc chuyên khoa nào (`CHUYÊNKHOA`).

Thuộc tính `VAITRÒ` nhận một trong các giá trị sau:

- `Điều phối viên`
- `Bác sĩ/Y sĩ`
- `Kỹ thuật viên`
- `Bệnh nhân`

#### HSBA

`HSBA(MÃHSBA, MÃBN, NGÀY, CHẨNĐOÁN, ĐIỀUTRỊ, MÃBS, MÃKHOA, KẾTLUẬN)`

Mỗi hồ sơ bệnh án (`HSBA`) có một mã duy nhất (`MÃHSBA`), liên quan đến một bệnh nhân (`MÃBN`), được lập vào một ngày (`NGÀY`), có chẩn đoán (`CHẨNĐOÁN`), hướng điều trị (`ĐIỀUTRỊ`) của y sĩ hoặc bác sĩ điều trị (`MÃBS`). Hồ sơ bệnh án thể hiện bệnh nhân được tiếp nhận khám và điều trị tại một khoa có mã là `MÃKHOA`, với kết luận của y sĩ hoặc bác sĩ điều trị là `KẾTLUẬN`.

#### HSBA_DV

`HSBA_DV(MÃHSBA, LOẠIDV, NGÀYDV, MÃKTV, KẾTQUẢ)`

Ghi nhận các dịch vụ hỗ trợ chẩn đoán (`LOẠIDV`) đã được thực hiện theo chỉ định của y sĩ hoặc bác sĩ điều trị, vào một ngày (`NGÀYDV`) liên quan đến một hồ sơ bệnh án (`MÃHSBA`), người thực hiện dịch vụ (`MÃKTV`) và kết quả (`KẾTQUẢ`).

#### ĐƠNTHUỐC

`ĐƠNTHUỐC(MÃHSBA, NGÀYĐT, TÊNTHUỐC, LIỀUDÙNG)`

Là đơn thuốc mà y sĩ hoặc bác sĩ điều trị cho bệnh nhân (qua `MÃHSBA`) đã chỉ định vào ngày (`NGÀYĐT`) gồm tên thuốc (`TÊNTHUỐC`) và liều dùng (`LIỀUDÙNG`).

### Mô tả hệ thống

Cơ sở dữ liệu được cài đặt trên Oracle. Hệ thống dùng chính sách đóng, tức người dùng `u` cần được cấp quyền `p` trên đối tượng dữ liệu `o` mới có thể thực hiện `p` trên `o`.

DBA trong hệ thống `S` thực hiện việc cấp quyền cho nhân sự trong toàn hệ thống theo mô tả như sau:

### TC#1

Ngoài DBA, tất cả người dùng trong hệ thống `S` gồm những nhân viên trong quan hệ `NHÂNVIÊN` và cả những bệnh nhân trong quan hệ `BỆNHNHÂN`.

DBA tạo tài khoản cho tất cả những người dùng này, và nhập liệu cho các bảng dữ liệu như `NHÂNVIÊN`. DBA không tự định nghĩa bảng (`table`) dùng để quản lý tài khoản người dùng mà sử dụng thông tin tài khoản do hệ quản trị CSDL Oracle quản lý.

Bằng cách nào DBA có thể kết nối một tên tài khoản với một dòng dữ liệu là người dùng tương ứng trong quan hệ `NHÂNVIÊN` và `BỆNHNHÂN` mà không phải truy cập dữ liệu từ nhiều hơn một bảng, đồng thời phải ép thỏa các chính sách bảo mật liên quan đến những người dùng này.

### TC#2

Có 20 nhân viên với vai trò `Điều phối viên`. Các nhân viên giữ vai trò này có thể:

- xem, thêm và sửa dữ liệu trên quan hệ `BỆNHNHÂN`
- tạo mới hồ sơ bệnh án (`HSBA`)
- điều phối y bác sĩ phụ trách hồ sơ bệnh án bằng cách cập nhật trường `MÃKHOA`, `MÃBS`
- điều phối kỹ thuật viên (`MÃKTV`) thực hiện các dịch vụ hỗ trợ chẩn đoán do bác sĩ chỉ định

### TC#3

Có 100 nhân viên với vai trò `Bác sĩ/Y sĩ`, có chức năng:

a. Xem các hồ sơ bệnh án mà bác sĩ hoặc y sĩ đó đã điều trị.  
b. Thêm, xóa dòng trên quan hệ `HSBA_DV`, là các dịch vụ cần thực hiện thêm liên quan hồ sơ bệnh án mà bác sĩ hoặc y sĩ phụ trách, giúp bác sĩ hoặc y sĩ có chẩn đoán chính xác trong quá trình điều trị bệnh.  
c. Cập nhật giá trị các trường `CHẨNĐOÁN`, `ĐIỀUTRỊ`, `KẾTLUẬN` liên quan các hồ sơ bệnh án mà bác sĩ hoặc y sĩ phụ trách. Các hành vi cập nhật trên các trường này đều được hệ thống ghi vết.  
d. Được xem danh sách bệnh nhân liên quan đến các hồ sơ bệnh án mà y sĩ hoặc bác sĩ đã điều trị. Được cập nhật giá trị các trường `TIỀNSỬBỆNH`, `TIỀNSỬBỆNHGD`, `DỊỨNGTHUỐC` của các bệnh nhân mà bác sĩ hoặc y sĩ điều trị.  
e. Thêm, xóa, cập nhật trên quan hệ `ĐƠNTHUỐC` liên quan đến các hồ sơ bệnh án mà y sĩ hoặc bác sĩ đó điều trị. Việc điều chỉnh đơn thuốc liên quan đến tên thuốc (`TÊNTHUỐC`), liều dùng (`LIỀUDÙNG`) sẽ được ghi vết sau khi đơn thuốc đã được tạo.

### TC#4

Có 50 nhân viên giữ vai trò `Kỹ thuật viên`. Các kỹ thuật viên thực hiện các dịch vụ theo chỉ định của bác sĩ và sự điều phối của điều phối viên, ghi kết quả tại trường `KẾTQUẢ` trong quan hệ `HSBA_DV`.

Các kỹ thuật viên chỉ có thể xem các dòng trong quan hệ `HSBA_DV` do mình được điều phối và thực hiện. Các thao tác cập nhật trên trường `KẾTQUẢ` đều được ghi vết.

### TC#5

Hệ thống hiện tại có khoảng 100000 người dùng là `Bệnh nhân`. Trên hệ thống `S`, trừ DBA, mỗi nhân viên hoặc bệnh nhân đăng nhập chỉ có thể xem thông tin của chính mình:

- trên bảng `NHÂNVIÊN` nếu là nhân viên
- trên bảng `BỆNHNHÂN` nếu là bệnh nhân

Đồng thời có thể chỉnh sửa các trường, trừ các trường liên quan mã, họ tên, phái, ngày sinh, CCCD, vai trò, chuyên khoa tùy quan hệ tương ứng, liên quan đến chính người đó.

## Yêu cầu 1: Giải pháp cấp quyền truy cập và cài đặt giao diện

### Câu 1

Em hãy cài đặt cơ sở dữ liệu và thiết lập tài khoản theo mô tả ở TC#1.

### Câu 2

Em hãy ép thỏa các chính sách bảo mật liên quan vai trò `Kỹ thuật viên` và `Bệnh nhân` dùng cơ chế `RBAC` theo mô tả và cài đặt giao diện cho những người dùng liên quan.

### Câu 3

Em hãy ép thỏa các chính sách bảo mật liên quan vai trò `Điều phối viên` và `Y sĩ/Bác sĩ` dùng cơ chế `VPD` theo mô tả và cài đặt giao diện cho những người dùng liên quan.

## Yêu cầu 2: Cơ chế phát tán thông báo dùng OLS và cài đặt giao diện

Dựa vào chuyên môn, giả sử hiện tại bệnh viện có 3 khoa:

- Khoa Tiêu hóa
- Khoa Thần kinh
- Khoa Tim mạch

Ngoài ra, bệnh viện có 3 cơ sở tại:

- Hồ Chí Minh
- Hải Phòng
- Hà Nội

Có sự phân chia vai trò người dùng theo 3 cấp bậc:

- Ban Giám đốc
- Lãnh đạo khoa
- Nhân viên

Bệnh viện cần gửi những dòng trong quan hệ `THÔNGBÁO`, gồm các trường `NỘIDUNG`, `NGÀYGIỜ`, `ĐỊAĐIỂM` về những cuộc họp khẩn đến các vai trò trong bệnh viện dùng cơ chế `OLS (Oracle Label Security)`.

Hãy thiết lập hệ thống nhãn gồm 3 thành phần, có thể điều chỉnh mô hình dữ liệu nếu cần thiết để hệ thống có thể đáp ứng các yêu cầu sau, đồng thời cài đặt giao diện minh họa trên ứng dụng.

### Người dùng cần gán nhãn

| Định danh | Mô tả |
|---|---|
| `u1` | Giám đốc có thể đọc được toàn bộ thông báo |
| `u2` | Lãnh đạo Khoa tim mạch tại Hồ Chí Minh |
| `u3` | Lãnh đạo Khoa thần kinh tại Hà Nội |
| `u4` | Nhân viên thuộc Khoa thần kinh tại Hồ Chí Minh |
| `u5` | Nhân viên thuộc Khoa tim mạch tại Hồ Chí Minh |
| `u6` | Lãnh đạo phòng có thể đọc các thông báo của Khoa tim mạch tại Hồ Chí Minh |
| `u7` | Lãnh đạo phòng có thể đọc được toàn bộ thông báo phù hợp với cấp lãnh đạo phòng |
| `u8` | Nhân viên thuộc Khoa Tiêu hóa tại Hà Nội |

### Dữ liệu

| Định danh | Mô tả |
|---|---|
| `t1` | Gửi đến toàn bộ nhân viên |
| `t2` | Gửi đến toàn bộ Ban giám đốc |
| `t3` | Gửi đến các lãnh đạo khoa |
| `t4` | Gửi đến lãnh đạo Khoa Tiêu hóa |
| `t5` | Gửi đến nhân viên Khoa Tiêu hóa ở Hồ Chí Minh |
| `t6` | Gửi đến nhân viên Khoa Tiêu hóa ở Hà Nội |
| `t7` | Gửi đến lãnh đạo Khoa Tiêu hóa và Khoa Thần kinh tại Hải Phòng |

## Yêu cầu 3: Vận dụng cơ chế kiểm toán

Sinh viên hãy thiết lập các yêu cầu kiểm toán như sau và đọc nhật ký kiểm toán ghi nhận được. Không cần cài đặt giao diện.

1. Kích hoạt kiểm toán hệ thống.
2. Thực hiện kiểm toán dùng `Standard Audit`: theo dõi hành vi của những user cụ thể trên những đối tượng cụ thể của cơ sở dữ liệu gồm `table`, `view`, `stored procedure`, `function`, có thiết lập theo dõi các hành vi thành công hay không thành công. Sinh viên tự đề nghị 5 ngữ cảnh khác nhau để thiết lập kiểm toán và kiểm chứng lại nhật ký kiểm toán.
3. Sinh viên có thể dùng `Fine-grained Audit` hoặc `Unified Audit` để thực hiện kiểm toán trong các tình huống sau và tạo tình huống để có dữ liệu nhật ký kiểm toán với các hành vi sau:
   - cập nhật trên thuộc tính `MÃHSBA`, `NGÀYĐT`, `TÊNTHUỐC`, `LIỀUDÙNG` của quan hệ `ĐƠNTHUỐC` của y sĩ hoặc bác sĩ điều trị sau khi đơn thuốc đã được chỉ định
   - hành vi của người dùng thuộc vai trò `Y sĩ/Bác sĩ` đã cập nhật thành công trên các trường `CHẨNĐOÁN`, `ĐIỀUTRỊ`, `KẾTLUẬN` của hồ sơ bệnh án (`HSBA`) mà y sĩ hoặc bác sĩ đó điều trị
   - hành vi của người dùng cập nhật bất hợp pháp trên các trường `CHẨNĐOÁN`, `ĐIỀUTRỊ`, `KẾTLUẬN`
   - hành vi thêm, xóa, sửa bất hợp pháp trên quan hệ `HSBA_DV`
4. Đọc xuất dữ liệu kiểm toán ở mỗi trường hợp.

## Yêu cầu 4: Sao lưu và phục hồi dữ liệu

Sinh viên hãy tìm hiểu về cơ chế sao lưu và phục hồi dữ liệu do các HQT CSDL cung cấp và cài đặt chức năng sao lưu, gồm chủ động và tự động, và khôi phục dữ liệu dựa vào nhật ký kiểm toán ở Yêu cầu 3 sau khi có sự cố. Với yêu cầu 4, không yêu cầu cài đặt giao diện.

1. Tìm hiểu các phương pháp thực hiện sao lưu và phục hồi dữ liệu.
2. Hiện thực các phương pháp đó trên HQT CSDL Oracle.
3. Đánh giá ưu khuyết điểm các phương pháp đã tìm hiểu và thử nghiệm.
4. Kết luận.

## Một số quy định

1. Nhóm phải thực hiện cả hai phân hệ, cùng một ứng dụng.
2. Kế hoạch chấm đồ án sẽ được thông báo cụ thể trên Moodle.
3. Cuốn báo cáo đồ án:
   - trình bày giải pháp lý thuyết ngắn gọn, dễ hiểu, ghi rõ tài liệu tham khảo, không dịch lại tài liệu, chủ yếu là phần tóm lược những gì tìm hiểu được, nhận xét, đánh giá, thuyết minh các kết quả đạt được
   - nhóm trưởng làm bảng phân công công việc và đánh giá thành viên trong nhóm, đóng chung trong cuốn báo cáo đồ án. Ghi rõ mỗi thành viên hoàn thành bao nhiêu phần trăm công việc được giao và mỗi thành viên đóng góp bao nhiêu phần trăm để hoàn thành đồ án. Giả sử mỗi phân hệ của đồ án ứng với 100% thì mỗi thành viên hoàn thành bao nhiêu phần trăm trong từng phân hệ
4. Nộp cuối kỳ:
   - bản in báo cáo trên giấy nộp vào ngày chấm đồ án, đồng thời cũng nộp trên Moodle trước deadline
   - gồm các tập tin MS Word báo cáo, source code, script CSDL gồm script schema và data. Tên tập tin đặt theo quy định là mã sinh viên của các thành viên trong nhóm, cách nhau bởi dấu `_`. Tất cả tập tin được lưu trong thư mục với tên theo quy định: `ATBM-2026-MãNhóm`
5. Tất cả các thành viên của nhóm đều cần có khả năng thực hiện các yêu cầu của đồ án. Bất kỳ sinh viên nào cũng có thể được giáo viên chấm đồ án yêu cầu thực hiện tại chỗ việc cài đặt một số chính sách bảo mật.
6. Bài giống nhau hoặc có copy hoặc sao chép: tất cả thành viên đều 0 điểm môn học.
