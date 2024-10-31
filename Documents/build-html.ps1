# HTMLとして出力する

asciidoctor ./API設計書/src/index.adoc -a stylesheet=wellshipstyle.css --destination-dir ./API設計書/dist/ -r asciidoctor-diagram
scp -r ./API設計書/dist/images aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\webapi-design
scp ./API設計書/dist/index.html aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\webapi-design\index.html
asciidoctor ./UI設計書/src/index.adoc -a stylesheet=wellshipstyle.css --destination-dir ./UI設計書/dist/ -r asciidoctor-diagram
scp -r ./UI設計書/dist/images aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\ui-design
scp ./UI設計書/dist/index.html aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\ui-design\index.html
asciidoctor ./アーキテクチャ設計書/src/index.adoc -a stylesheet=wellshipstyle.css --destination-dir ./アーキテクチャ設計書/dist/ -r asciidoctor-diagram
scp -r ./アーキテクチャ設計書/dist/images aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\architecture
scp ./アーキテクチャ設計書/dist/index.html aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\architecture\index.html
