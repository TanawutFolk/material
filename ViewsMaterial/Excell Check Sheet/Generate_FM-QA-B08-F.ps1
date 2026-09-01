param(
    [string]$OutputPath = (Join-Path $PSScriptRoot 'FM-QA-B08-F Receiving Inspection Check Sheet.xlsx')
)

# สร้างฟอร์มเปล่า FM-QA-B08-F ตามต้นฉบับจริง : มุมซ้ายบน B2 มุมขวาล่าง AE66
# โครงนี้ใช้เป็นสเปกให้ ExportExcellB08.cs วาดตาม
#
# ผังคอลัมน์
#   B..D   ป้ายหัวข้อด้านซ้าย
#   E..M   Content / criteria
#   N..P   Method / Equipment
#   Q..AE  Judgement / ช่องข้อมูล (cavity 5 ช่อง : Q:S T:V W:Y Z:AB AC:AE)

$ErrorActionPreference = 'Stop'

function Get-RgbColor([int]$red, [int]$green, [int]$blue) {
    return $red + (256 * $green) + (65536 * $blue)
}

$colorWhite  = Get-RgbColor 255 255 255
$colorBlack  = Get-RgbColor 0 0 0
$colorBlue   = Get-RgbColor 0 0 255
$colorRed    = Get-RgbColor 255 0 0
$colorYellow = Get-RgbColor 255 192 0
$colorGreen  = Get-RgbColor 204 255 204  # ช่องหัวข้อ #CCFFCC
$colorGray   = Get-RgbColor 191 191 191

# ช่องกรอกข้อมูล = ขาว
$colorInput = $colorWhite

# บล็อกตรวจสอบ (Regular / Function / Dimension / Appearance) ตั้งต้นเป็นเทา
# ตอน export จริง บล็อกไหนที่ M-CODE ต้องตรวจจะถูกเปลี่ยนเป็นขาว
$colorCheck = $colorGray

$xlCenter = -4108
$xlLeft   = -4131
$xlRight  = -4152
$xlTop    = -4160
$xlPortrait = 1
$xlPaperA4  = 9
$xlOpenXmlWorkbook = 51
$xlContinuous = 1
$xlThin   = 2
$xlMedium = -4138

$excel = $null
$workbook = $null
$sheet = $null

# WrapText ปิดเป็นค่าเริ่มต้น ต้นฉบับเปิดเฉพาะช่องข้อความยาวไม่กี่ช่อง
# ถ้าเปิดทั่วไป ข้อความจะถูกยัดในคอลัมน์กว้าง 2.86 แล้วเละ
function Set-Block {
    param(
        [Parameter(Mandatory = $true)][string]$Address,
        [AllowEmptyString()][string]$Text = '',
        [int]$FillColor = $colorWhite,
        [int]$FontColor = $colorBlack,
        [double]$FontSize = 9,
        [bool]$Bold = $false,
        [bool]$Italic = $false,
        [int]$HorizontalAlignment = $xlCenter,
        [int]$VerticalAlignment = $xlCenter,
        [bool]$WrapText = $false,
        [bool]$Border = $true
    )

    $range = $script:sheet.Range($Address)
    if ($Address.Contains(':')) {
        $range.Merge()
    }

    $range.Value2 = $Text
    $range.Interior.Color = $FillColor
    $range.Font.Name = 'Tahoma'
    $range.Font.Size = $FontSize
    $range.Font.Bold = $Bold
    $range.Font.Italic = $Italic
    $range.Font.Color = $FontColor
    $range.HorizontalAlignment = $HorizontalAlignment
    $range.VerticalAlignment = $VerticalAlignment
    $range.WrapText = $WrapText
    $range.ShrinkToFit = $false

    if ($Border) {
        $range.Borders.LineStyle = $xlContinuous
        $range.Borders.Weight = $xlThin
        $range.Borders.Color = $colorBlack
    }

    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($range)
}

function Set-OuterBorder([string]$Address, [int]$Color, [int]$Weight) {
    $range = $script:sheet.Range($Address)
    foreach ($borderIndex in 7, 8, 9, 10) {
        $border = $range.Borders.Item($borderIndex)
        $border.LineStyle = $xlContinuous
        $border.Weight = $Weight
        $border.Color = $Color
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($border)
    }
    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($range)
}

# เพิ่มช่องติ๊กแบบ Form Control ชิดขวาของช่องที่ระบุ
# ใช้ Form Control ไม่ใช่ ActiveX เพราะบันทึกใน .xlsx ได้และไม่ต้องเปิด macro
function Add-CheckBox([string]$Address, [string]$Caption, [bool]$AlignLeft = $false) {
    $range = $script:sheet.Range($Address)

    $maxWidth = if ($AlignLeft) { 140 } else { 26 }
    $boxWidth  = [Math]::Min($range.Width - 2, $maxWidth)
    $boxHeight = [Math]::Min($range.Height - 1, 13)
    $left = if ($AlignLeft) { $range.Left + 3 } else { $range.Left + $range.Width - $boxWidth - 1 }
    $top  = $range.Top + (($range.Height - $boxHeight) / 2)

    $box = $script:sheet.CheckBoxes().Add($left, $top, $boxWidth, $boxHeight)
    $box.Caption = $Caption
    $box.Value = 0
    $box.Display3DShading = $false

    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($box)
    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($range)
}

# ลบเส้นคั่นแนวนอนระหว่าง 2 ช่องที่ติดกันบน-ล่าง
# ต้องลบทั้งขอบล่างของตัวบนและขอบบนของตัวล่าง เพราะ Excel ใช้เส้นร่วมกัน
function Remove-HorizontalDivider([string]$UpperAddress, [string]$LowerAddress) {
    $upper = $script:sheet.Range($UpperAddress)
    $upper.Borders.Item(9).LineStyle = -4142
    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($upper)

    $lower = $script:sheet.Range($LowerAddress)
    $lower.Borders.Item(8).LineStyle = -4142
    [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($lower)
}

# แถวช่อง cavity 5 ช่อง : Q:S  T:V  W:Y  Z:AB  AC:AE
function Set-CavityRow([int]$Row, [string]$Text, [int]$FillColor) {
    foreach ($pair in @(@('Q','S'), @('T','V'), @('W','Y'), @('Z','AB'), @('AC','AE'))) {
        Set-Block "$($pair[0])$Row`:$($pair[1])$Row" $Text $FillColor $colorBlack 9
    }
}

try {
    $outputDirectory = Split-Path -Parent $OutputPath
    if (-not (Test-Path -LiteralPath $outputDirectory)) {
        [void](New-Item -ItemType Directory -Path $outputDirectory -Force)
    }

    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    $excel.DisplayAlerts = $false
    $excel.ScreenUpdating = $false

    $workbook = $excel.Workbooks.Add()
    $sheet = $workbook.Worksheets.Item(1)
    $script:sheet = $sheet
    $sheet.Name = 'Master'

    $sheet.Cells.Font.Name = 'Tahoma'
    $sheet.Cells.Font.Size = 9
    $sheet.Cells.VerticalAlignment = $xlCenter
    $sheet.Cells.WrapText = $false

    # ความกว้างคอลัมน์ : A เป็นขอบซ้าย , B..J = 2.86 , K = 4.57 , L..P = 2.86 , Q..AE = 2.71
    $sheet.Columns.Item(1).ColumnWidth = 2.0
    for ($c = 2; $c -le 31; $c++) {
        $width = if ($c -eq 11) { 4.57 } elseif ($c -le 16) { 2.86 } else { 2.71 }
        $sheet.Columns.Item($c).ColumnWidth = $width
    }

    # ความสูงแถวตามต้นฉบับ (index 0 = แถว 2)
    $rowHeights = @(
        18, 18, 12.8, 15, 15, 3.8, 15, 15,          # R2-R9
        12.8, 12.8, 13.5, 12.8, 13.5, 12.8,         # R10-R15 บรรจุภัณฑ์
        15,                                          # R16 Lot No.
        12.8, 12.8,                                  # R17-R18 Regular
        12.8, 15, 15,                                # R19-R21 Function
        15.8, 15.8, 15.8, 15.8, 15.8, 15.8, 15.8, 15.8,   # R22-R29 Dimension
        15.8, 15.8, 15.8, 15.8, 15.8, 15.8,         # R30-R35 Appearance
        14.2, 14.2, 14.2                             # R36-R38
    )
    for ($i = 0; $i -lt $rowHeights.Count; $i++) {
        $sheet.Rows.Item($i + 2).RowHeight = [double]$rowHeights[$i]
    }
    for ($r = 39; $r -le 58; $r++) { $sheet.Rows.Item($r).RowHeight = [double]14.2 }
    $sheet.Rows.Item(59).RowHeight = [double]15
    for ($r = 60; $r -le 64; $r++) { $sheet.Rows.Item($r).RowHeight = [double]12.8 }
    $sheet.Rows.Item(65).RowHeight = [double]15
    $sheet.Rows.Item(66).RowHeight = [double]15

    # ---------- R2-R4 : หัวเอกสาร ----------
    Set-Block 'B2'      'FM-QA-B08-F Receiving Inspection Check Sheet' $colorWhite $colorBlack 14 $true $false $xlLeft $xlCenter $false $false
    Set-Block 'V2:AA2'  'Report No.' $colorGreen $colorBlack 10
    Set-Block 'AB2:AE2' 'Approve'    $colorGreen $colorBlack 10

    Set-Block 'B3:G3'   'M-Code' $colorInput $colorBlue 14 $false $true $xlLeft $xlCenter $false $false
    Set-Block 'H3:U3'   'Material Name' $colorInput $colorBlue 14 $false $true $xlLeft $xlCenter $false $false
    Set-Block 'V3:AA4'  'Report No.' $colorInput $colorBlue 18 $false $false $xlCenter

    Set-Block 'B4:J4'   '' $colorWhite $colorBlack 10 $false $false $xlLeft $xlCenter $false $false
    Set-Block 'AB3:AE4' '' $colorWhite   # ช่องแปะรูป Stamp

    # ---------- R5-R6 : ข้อมูลการรับเข้า ----------
    Set-Block 'B5:D5'   'Receive Date' $colorGreen $colorBlack 8 $true
    Set-Block 'E5:K5'   'วันเดือนปีที่รับเข้า' $colorInput $colorBlue 11 $false $false $xlLeft
    Set-Block 'L5:N6'   'INV. No.' $colorGreen $colorBlack 10 $true $false $xlCenter $xlCenter $true
    Set-Block 'O5:U5'   'หมายเลขอินวอย' $colorInput $colorBlue 11 $false $false $xlLeft
    Set-Block 'V5:X5'   'Lot Size.' $colorGreen $colorBlack 10 $true
    Set-Block 'Y5:AC5'  'จำนวนทั้งหมด' $colorInput $colorBlue 9
    Set-Block 'AD5:AE5' 'Pcs' $colorWhite $colorBlack 9

    Set-Block 'B6:D6'   'Vender' $colorGreen $colorBlack 9 $true
    Set-Block 'E6:K6'   '' $colorInput $colorBlue 9 $false $false $xlLeft
    Set-Block 'O6:U6'   '' $colorWhite $colorBlack 9
    Set-Block 'V6:X6'   'Issue by' $colorGreen $colorBlack 9 $true
    Set-Block 'Y6:AA6'  'O/P WH' $colorWhite $colorBlue 9
    Set-Block 'AB6:AE6' 'Issue Time' $colorWhite $colorBlue 8

    # R7 เป็นแถวคั่นบาง ๆ ไม่มีเส้น

    # ---------- R8-R9 : ผู้ตรวจ + หัวตาราง ----------
    Set-Block 'B8:D8'   'Ins. Date' $colorGreen $colorBlack 9 $true
    Set-Block 'E8:M8'   '' $colorWhite $colorBlue 9
    Set-Block 'N8:P8'   'Inspector' $colorGreen $colorBlack 10 $true
    Set-Block 'Q8:AE8'  '' $colorWhite $colorBlue 9 $false $false $xlLeft

    Set-Block 'B9:D9'   'Item'      $colorGreen $colorBlack 10 $true
    Set-Block 'E9:M9'   'Content'   $colorGreen $colorBlack 10 $true
    Set-Block 'N9:P9'   'Method'    $colorGreen $colorBlack 10 $true
    Set-Block 'Q9:AE9'  'Judgement' $colorGreen $colorBlack 10 $true

    # ---------- R10-R15 : บรรจุภัณฑ์ ----------
    Set-Block 'B10:D15' 'บรรจุภัณฑ์' $colorWhite $colorBlack 9 $false $false $xlCenter $xlCenter $true
    Set-Block 'N10:P15' 'ตาเปล่า'    $colorWhite $colorBlack 9 $false $false $xlCenter $xlCenter $true

    Set-Block 'E10:M11' 'กล่อง/ถุง อยู่ในสภาพสมบูรณ์ไม่บุบ ยุบหรือฉีกขาด' $colorWhite $colorBlack 9 $false $false $xlLeft $xlCenter $true
    Set-Block 'E12:M13' 'ชื่อของชิ้นงานที่ได้รับตรงกับชิ้นงานจริงในกล่องและตรงกับป้ายแสดงข้างกล่อง' $colorWhite $colorBlack 9 $false $false $xlLeft $xlCenter $true
    Set-Block 'E14:M15' 'จำนวนที่ได้รับตรงกับจำนวนที่แสดงในช่อง Lot Size และตรงกับป้ายแสดงข้างกล่อง' $colorWhite $colorBlack 9 $false $false $xlLeft $xlCenter $true

    Set-Block 'Q10:AE10' ''          $colorInput $colorBlue 9 $false $false $xlLeft
    Set-Block 'Q11:AE11' 'อาการ NG'  $colorInput $colorBlack 8 $false $false $xlLeft
    Set-Block 'Q12:AE12' ''          $colorInput $colorBlue 9 $false $false $xlLeft
    Set-Block 'Q13:AE13' 'อาการ NG'  $colorInput $colorBlack 8 $false $false $xlLeft
    Set-Block 'Q14:S15'  'ขนาดบรรจุ' $colorInput $colorBlack 8 $false $false $xlLeft
    Set-Block 'T14:AE14' ''          $colorInput $colorBlue 9
    Set-Block 'T15:AE15' ''          $colorInput $colorBlue 9

    # ช่องผลตัดสินกับบรรทัด 'อาการ NG' ใต้มันเป็นกล่องเดียวกัน ไม่ต้องมีเส้นคั่น
    Remove-HorizontalDivider 'Q10:AE10' 'Q11:AE11'
    Remove-HorizontalDivider 'Q12:AE12' 'Q13:AE13'
    Remove-HorizontalDivider 'T14:AE14' 'T15:AE15'

    # ---------- R16 : Lot No. ----------
    Set-Block 'B16:D16' 'Lot No.' $colorWhite $colorBlack 9
    Set-Block 'E16:AE16' ''       $colorInput $colorBlue 9 $false $false $xlLeft

    # ---------- R17-R18 : Regular (สรุปเท่านั้น รายละเอียดอยู่ใน FM-QA-B13-A) ----------
    Set-Block 'B17:D17' 'Regular'    $colorCheck $colorBlue 9 $false $false $xlCenter $xlCenter $true
    Set-Block 'B18:D18' 'Inspection' $colorCheck $colorBlue 9
    Set-Block 'E17:P18' '{Regular criteria}' $colorCheck $colorBlue 9 $false $false $xlLeft $xlTop
    Set-Block 'Q17:T17' 'Regular check' $colorCheck $colorBlue 9 $false $false $xlLeft
    Set-Block 'U17:AE17' ''             $colorCheck $colorBlue 9 $false $false $xlLeft
    Set-Block 'Q18:T18' "Scrap Q'ty :"  $colorCheck $colorBlue 9 $false $false $xlLeft
    Set-Block 'U18:AE18' ''             $colorCheck $colorBlue 9 $false $false $xlLeft

    # ---------- R19-R21 : Function ----------
    Set-Block 'B19:D19' 'Function'         $colorCheck $colorBlack 9
    Set-Block 'B20:D21' 'Inspection Level' $colorCheck $colorBlack 9 $false $false $xlCenter $xlCenter $true
    Set-Block 'E19:M20' '{Function criteria}' $colorCheck $colorBlue 9 $false $false $xlLeft $xlTop
    Set-Block 'N19:P20' '{Equipment}'         $colorCheck $colorBlack 9 $false $false $xlCenter $xlCenter $true
    Set-CavityRow 19 'Cavity'  $colorCheck
    Set-CavityRow 20 'OK . NG' $colorCheck
    Set-Block 'E21:P21' 'Function Judgement' $colorCheck $colorBlack 9
    Set-Block 'Q21:AE21' 'Accept  .  Reject' $colorCheck $colorBlack 9

    # ---------- R22-R29 : Dimension ----------
    Set-Block 'B22:D22' 'Dimension' $colorCheck $colorBlack 10
    Set-Block 'E22:P22' '{Equipment} SN :' $colorCheck $colorBlack 9 $false $false $xlLeft
    Set-Block 'Q22:AE22' '' $colorCheck

    Set-Block 'B23:D29' 'Inspection Level' $colorCheck $colorBlack 10 $false $false $xlCenter $xlCenter $true
    Set-Block 'E23:M27' '{Dimension criteria}' $colorCheck $colorBlue 9 $false $false $xlLeft $xlTop
    Set-Block 'N23:P27' '{Equipment}'          $colorCheck $colorBlack 9 $false $false $xlCenter $xlCenter $true

    Set-CavityRow 23 'Cavity' $colorCheck
    foreach ($r in 24, 25, 26) {
        Set-CavityRow $r '' $colorCheck
    }
    Set-CavityRow 27 'OK . NG' $colorCheck

    Set-Block 'E28:P28'  'ผลการวัดที่ได้จากผู้ผลิตต้องผ่านเกณฑ์' $colorYellow $colorBlack 9
    Set-Block 'Q28:AE28' 'Accept  .  Reject' $colorYellow $colorBlack 9
    Set-Block 'E29:P29'  'Dimension Judgement' $colorYellow $colorBlack 9
    Set-Block 'Q29:AE29' 'Accept  .  Reject'   $colorYellow $colorBlack 9

    # ---------- R30-R36 : Appearance ----------
    Set-Block 'B30:D30' 'Appearance' $colorCheck $colorBlack 10
    Set-Block 'B31:D36' 'Inspection Level' $colorCheck $colorBlack 10 $false $false $xlCenter $xlCenter $true
    Set-Block 'E30:M35' '{Appearance criteria}' $colorCheck $colorBlue 9 $false $false $xlLeft $xlTop
    Set-Block 'N30:P35' '{Equipment}' $colorCheck $colorBlack 9 $false $false $xlCenter $xlCenter $true
    Set-Block 'Q30:AE30' "Inspection Q'ty :" $colorCheck $colorBlack 10 $false $false $xlLeft

    Set-Block 'Q31:S31'  'Date'        $colorCheck $colorBlack 9
    Set-Block 'T31:V31'  'Ope.'        $colorCheck $colorBlack 9
    Set-Block 'W31:Y31'  "Check Q'ty"  $colorCheck $colorBlack 9
    Set-Block 'Z31:AB31' 'OK'          $colorCheck $colorBlack 9
    Set-Block 'AC31:AE31' 'Pending'    $colorCheck $colorBlack 9

    foreach ($r in 32, 33, 34, 35) {
        Set-Block "Q$r`:S$r"   '' $colorCheck $colorBlue 9
        Set-Block "T$r`:V$r"   '' $colorCheck $colorBlue 9 $false $false $xlLeft
        Set-Block "W$r`:Y$r"   '' $colorCheck $colorBlue 9
        Set-Block "Z$r`:AB$r"  '' $colorCheck $colorBlue 9 $false $false $xlLeft
        Set-Block "AC$r`:AE$r" '' $colorCheck $colorBlue 9 $false $false $xlLeft

        # ช่องติ๊กจริง กดได้ ไม่ใช่ตัวอักษร ☐
        Add-CheckBox "T$r`:V$r"   'OK'
        Add-CheckBox "Z$r`:AB$r"  'P'
        Add-CheckBox "AC$r`:AE$r" 'P'
    }

    Set-Block 'E36:P36'  'Appearance Judgement' $colorCheck $colorBlack 9
    Set-Block 'Q36:AE36' 'Accept  .  Reject'    $colorCheck $colorBlack 9

    # ---------- R37-R38 : สรุปผล + หมายเหตุ ----------
    Set-Block 'B37:P37'  'Final Judgement :  {Condition}' $colorWhite $colorBlack 10 $false $false $xlLeft
    Set-Block 'Q37:AE37' 'OK (        Pcs).      Pending (        Pcs).      Scrap (        Pcs).' $colorWhite $colorBlack 10 $false $false $xlLeft

    Set-Block 'B38:E38'  'Check point :' $colorWhite $colorBlue 10 $false $false $xlLeft
    Set-Block 'F38:AE38' 'ทำการตรวจสอบชิ้นงานตัวแรก และทำเครื่องหมาย ☑ OK ที่ช่อง Ope. ถ้าชิ้นงานมีลักษณะตรงตามจุดเช็คที่กำหนด' $colorWhite $colorRed 9 $false $false $xlLeft

    # ---------- R39-R60 : พื้นที่แปะรูป / อ้างอิง (ช่องเดียวไม่มีเส้นแบ่งข้างใน) ----------
    Set-Block 'B39:AE60' 'Refer ST-QA-B30- Vender Inspection Report List' $colorWhite $colorBlack 10

    # ---------- R61-R66 : Pending detail ----------
    Set-Block 'B61:I61'   'Pending detail' $colorGreen $colorBlack 9
    Set-Block 'J61:K61'   "Q'ty"    $colorGreen $colorBlack 9
    Set-Block 'L61:M61'   "OK Q'ty" $colorGreen $colorBlack 9
    Set-Block 'N61:O61'   "NG Q'ty" $colorGreen $colorBlack 9
    Set-Block 'P61:W61'   'Pending detail' $colorGreen $colorBlack 9
    Set-Block 'X61:Y61'   "Q'ty"    $colorGreen $colorBlack 9
    Set-Block 'Z61:AA61'  "OK Q'ty" $colorGreen $colorBlack 9
    Set-Block 'AB61:AC61' "NG Q'ty" $colorGreen $colorBlack 9
    Set-Block 'AD61:AE61' 'NCR'     $colorGreen $colorBlack 9

    # NCR เป็นช่องเดียวคร่อม 3 แถว
    Set-Block 'AD62:AE64' '' $colorWhite

    for ($r = 62; $r -le 64; $r++) {
        $leftNumber  = $r - 61
        $rightNumber = $r - 58

        Set-Block "B$r"        "$leftNumber"  $colorWhite $colorBlack 9
        Set-Block "C$r`:I$r"   ''             $colorWhite $colorBlue 9 $false $false $xlLeft
        Set-Block "J$r`:K$r"   ''             $colorWhite $colorBlue 9
        Set-Block "L$r`:M$r"   ''             $colorWhite $colorBlue 9
        Set-Block "N$r`:O$r"   ''             $colorWhite $colorBlue 9
        Set-Block "P$r"        "$rightNumber" $colorWhite $colorBlack 9
        Set-Block "Q$r`:W$r"   ''             $colorWhite $colorBlue 9 $false $false $xlLeft
        Set-Block "X$r`:Y$r"   ''             $colorWhite $colorBlue 9
        Set-Block "Z$r`:AA$r"  ''             $colorWhite $colorBlue 9
        Set-Block "AB$r`:AC$r" ''             $colorWhite $colorBlue 9

    }

    Set-Block 'B65:W65'   'Total' $colorWhite $colorBlack 9 $false $false $xlRight
    Set-Block 'X65:Y65'   ''      $colorWhite $colorBlue 9
    Set-Block 'Z65:AA65'  ''      $colorWhite $colorBlue 9
    Set-Block 'AB65:AC65' ''      $colorWhite $colorBlue 9
    Set-Block 'AD65:AE65' ''      $colorWhite $colorBlack 8
    Add-CheckBox 'AD65:AE65' 'P'

    Set-Block 'B66:T66'   ''  $colorWhite
    Add-CheckBox 'B66:T66' 'PRONESS Record' $true
    Set-Block 'U66:Y66'   'Judgement by' $colorWhite $colorBlack 10
    Set-Block 'Z66:AA66'  'Date'         $colorWhite $colorBlack 10
    Set-Block 'AB66:AE66' ''             $colorWhite

    # กรอบนอกของฟอร์ม
    Set-OuterBorder 'B2:AE66' $colorBlue $xlMedium

    $sheet.PageSetup.PrintArea = '$B$2:$AE$66'
    $sheet.PageSetup.Orientation = $xlPortrait
    $sheet.PageSetup.PaperSize = $xlPaperA4
    $sheet.PageSetup.Zoom = $false
    $sheet.PageSetup.FitToPagesWide = 1
    $sheet.PageSetup.FitToPagesTall = 1
    $sheet.PageSetup.CenterHorizontally = $true
    $sheet.PageSetup.CenterVertically = $false
    $sheet.PageSetup.LeftMargin   = $excel.InchesToPoints(0.2)
    $sheet.PageSetup.RightMargin  = $excel.InchesToPoints(0.2)
    $sheet.PageSetup.TopMargin    = $excel.InchesToPoints(0.25)
    $sheet.PageSetup.BottomMargin = $excel.InchesToPoints(0.25)
    $sheet.PageSetup.PrintGridlines = $false
    $sheet.DisplayPageBreaks = $false

    $sheet.Activate()
    $excel.ActiveWindow.DisplayGridlines = $false
    $excel.ActiveWindow.Zoom = 75
    $sheet.Range('B2').Select()

    if (Test-Path -LiteralPath $OutputPath) {
        Remove-Item -LiteralPath $OutputPath -Force
    }

    $workbook.SaveAs($OutputPath, $xlOpenXmlWorkbook)
    $workbook.Close($false)
    $workbook = $null
    Write-Output $OutputPath
}
finally {
    if ($workbook -ne $null) {
        try { $workbook.Close($false) } catch { }
    }
    if ($excel -ne $null) {
        try { $excel.Quit() } catch { }
    }
    if ($sheet -ne $null) {
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($sheet)
    }
    if ($workbook -ne $null) {
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($workbook)
    }
    if ($excel -ne $null) {
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel)
    }
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
}
