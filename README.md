#### 服務入口(URI)

- Swagger : <http://localhost:7260/swagger/index.html>
- Web Service SOAP : <http://localhost:7260/iiot/service.asmx>
- Web Socket MQTT : <ws://localhost:7260/iiot/service.mqtt>
- OPC UA : <opc.tcp://localhost:4840/machine/data-platform>

#### 默認埠號

- HTTP : 7260
- MQTT : 1883
- OPC UA : 4840

#### 環境需求

- Debian 12 (Bookworm)
- Microsoft .NET 8.0 SDK
- PostgreSQL 16
- InfluxDB 2.7.3

#### 環境建置

```Bash
# 建立工作區
sudo mkdir /home/iiot-app && cd /home/iiot-app

# 安裝 Microsoft 套件
sudo wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && / 
sudo dpkg -i packages-microsoft-prod.deb && sudo rm packages-microsoft-prod.deb && sudo apt -y update

# 安裝 Git 套件
sudo apt -y install git && git --version

# 安裝 .NET 8.0 SDK 套件
sudo apt -y install dotnet-sdk-8.0 && dotnet --info
```

---

#### 安裝資料庫(PostgreSQL 16)

```Bash
# 建立儲存倉庫
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list' && / 
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -

# 安裝程式套件
sudo apt -y install postgresql-15 && sudo systemctl start postgresql && / 
sudo systemctl enable postgresql && sudo systemctl status postgresql

# 設置資料庫密碼
sudo passwd postgres
```

###### 初始配置

```Bash
# 切換使用者
sudo -i -u postgres

# 進入 PostgreSQL shell
psql

# 建立資料庫
CREATE DATABASE majors;

# 新增使用者
CREATE ROLE ffg SUPERUSER LOGIN PASSWORD 'iMDS@30878408';

# 資料庫綁定使用者
GRANT ALL PRIVILEGES ON DATABASE majors TO ffg;

# 離開 PostgreSQL shell
\q
```

---

#### 安裝資料庫(InfluxDB 2.7.3)

```Bash
# 安裝程式套件
sudo wget https://dl.influxdata.com/influxdb/releases/influxdb2-2.7.3-linux-amd64.tar.gz && / 
sudo tar xvzf influxdb2-2.7.1-linux-amd64.tar.gz && sudo rm influxdb2-2.7.3-linux-amd64.tar.gz

# 添加環境變量
sudo cp influxdb2_linux_amd64/influxd /usr/local/bin/ && sudo rm -rf influxdb2_linux_amd64
```

###### Unit 文件內容

```Bash
[Unit]
Description=IIoT InfluxDB

[Service]
ExecStart=/usr/local/bin/influxd
User=ffg
RestartSec=10
Restart=always
KillSignal=SIGINT
SyslogIdentifier=iiot-influxdb

[Install]
WantedBy=multi-user.target
```

###### 後台進程(systemd service)

```Bash
# 建立 Unit 文件
cd /lib/systemd/system && sudo vi influxdb.service

# 啟動後台進程
sudo systemctl start influxdb && sudo systemctl enable influxdb && sudo systemctl status influxdb
```

###### 安裝命令行界面(Influx CLI)

```Bash
# 安裝程式套件
sudo wget https://dl.influxdata.com/influxdb/releases/influxdb2-client-2.7.3-linux-amd64.tar.gz && / 
sudo tar xvzf influxdb2-client-2.7.3-linux-amd64.tar.gz && sudo rm influxdb2-client-2.7.3-linux-amd64.tar.gz

# 添加環境變量
sudo cp influx /usr/local/bin/ && sudo rm influx && sudo rm LICENSE && sudo rm README.md
```

###### 初始配置

```Bash
# 依次輸入: 用戶名稱、用戶密碼、組織名稱、桶名稱、數據保存時間(單位:秒, 0表示永久保存)
$ influx setup --host http://localhost:8086
---------------------------------------------------
? Please type your primary username 'ffg'
? Please type your password 'iMDS@30878408'
? Please type your password again 'iMDS@30878408'
? Please type your primary organization name 'FFG-iMDS'
? Please type your primary bucket name 'base_systems'
? Please type your retention period in hours, or 0 for infinite '720'
```

---

#### 應用程式佈署

```Bash
# 拷貝原始代碼
cd /home/iiot-app && sudo git clone https://imds.mynetgear.com:53080/elihu/IaaS.Embedded.SMB.git

# 建置應用程式
cd ./IaaS.Embedded.SMB/4.1.Platforms/IIoT.Platform.Station && / 
sudo dotnet publish -c Release -r linux-x64 --no-self-contained -o /home/iiot-app/.project && / 
sudo rm -rf /home/iiot-app/IaaS.Embedded.SMB
```

###### Unit 文件內容

```Bash
[Unit]
Description=IIoT Platform

[Service]
WorkingDirectory=/home/iiot-app/.project
ExecStart=/usr/bin/dotnet /home/iiot-app/.project/IIoT.Platform.Station.dll
User=iiot-app
RestartSec=10
Restart=always
KillSignal=SIGINT
SyslogIdentifier=iiot-platform
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

###### 後台進程(systemd service)

```Bash
# 建立 Unit 文件
cd /lib/systemd/system && sudo vi iiot-app.service

# 啟動服務
sudo systemctl start iiot-app && sudo systemctl enable iiot-app && sudo systemctl status iiot-app
```