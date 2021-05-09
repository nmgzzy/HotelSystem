# 旅店管理系统



使用说明：
顶部为菜单，左侧为数据显示区，右侧为数据修改区，底部显示当日剩余房间等信息。

登录：
--输入用户名、密码、授权码即可登录，系统会自动记住有效的授权码，授权码到期后需再次购买。注册新用户时需输入两次密码，建议用户名和密码采用6-20位英文字母+数字的组合，不可过长或过短。

顾客管理：
--预定录入
	将预定信息录入系统，必需填写姓名、身份证号、入住日期、离店日期、房间号，其他信息可以填写，也可等入住的时候填写。
--取消预定
	删除预定信息，必需填写身份证号。其他不必填写，如果需要修改预定天数，可以先查询，确定已定天数，删除原数据，重新预定录入。
--查询数据
	默认过去30天至未来30天入住旅客的信息，如需查询指定时间段，请在日期栏填写查询的开始时间和结束时间，然后点击查询数据按钮。点击数据某一列的标题可对数据进行排序。
--导出数据
	需要先进行查询，查询后单击导出数据按钮导出CSV文件，可用Excel打开并另存为xsl文件。
--确认入住
	顾客到店后输入身份证号、房间号、床位号，点击确认入住即可。

房间管理：
--增加房间
	输入房间号、房间名、床位数量、房间类型，点击增加房间。
--删除房间
	输入房间号，点击删除房间。
--查询所有房间
	点击查看所有房间信息。
--查询入住信息
	设置查查询的开始和结束时间，点击查询入住信息按钮即可查看对应的入住情况，点击列标题可以进行排序，可导出表格。未设置床位号的旅客名字不会显示在表格中，“未选床位人数”一栏中显示当天未设置床位的旅客人数。
--导出数据
	需要先进行查询，查询后单击导出数据按钮导出屏幕上的表格。

管理员账户：功能开发中。。。