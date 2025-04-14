# A5M2コマンドラインユーティリティでHTMLのテーブル定義書とPDFのER図を出力します。
# 前提：a5m2cmdにPATHを通している

# テーブル定義書
mkdir definition
Set-Location definition
a5m2cmd /Definition /Connect="__ConnectionType=Internal;ProviderName=PostgreSQL;SavePassword=True;ServerName=localhost;Port=15432;Database=stg01;UserName=stg01_ops;Password=stg01_p@ssw0rd;DBType=PostgreSQL;ProtocolVersion=30;SSLMode=Prefer;SSLCACert=C:\Users\r30370\Documents\Projects\2024-WellShip\aws\rds-ca-2015-root.pem;SSLIgnoreServerCertificateValidity=True;SSLIgnoreServerCertificateConstraints=True;SSLTrustServerCertificate=True" /SchemaName=resultcollector /SystemName=WELLSHIP /SubSystemName=結果収集 /Author=両備システムズヘルスケアソリューションカンパニーウェルネスビジネス事業部 /WithSchema=False

Set-Location ..
scp -r ./definition aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\database
Remove-Item -Recurse -Force ./definition

# ER図
mkdir erd
Set-Location erd
a5m2cmd /ERPDF /ERD="..\..\Database\ERD\ResultCollector.a5er" /PDF="wellship-erd-both.pdf" /View=Both
Set-Location ..
scp -r ./erd aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\database
Remove-Item -Recurse -Force ./erd
