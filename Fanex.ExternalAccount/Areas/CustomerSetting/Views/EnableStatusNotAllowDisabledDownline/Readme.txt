0. Config:
<configSections>
	<section name="enablestatus" type="Fanex.CustomerSetting.EnableStatus.AllInOne.Mappings.EnableStatusConfiguration, Fanex.CustomerSetting.EnableStatus.AllInOne, Version=1.0.0.0, Culture=neutral" />
</configSections>
<enablestatus configSource="Areas\CustomerSetting\Views\EnableStatus\Templates\Products.config" />

URL to call get and update:
1. Get: customersetting/enablestatus/productstatus/index?context={0}
	{0}: 
	context = {
		"username": customer name need to edit (in case of enable/disable status without bet type id),
		"synced": for bingo, casino only,
		"isCasino": true if enable casino is used,
		"hideDownline": true if you wanna update status without downline is mentioned,
		"enable": true/false (in case of enable/disable status without bet type id),
		"enableUplines": true/false (in case of enable/disable status without bet type id),
		"enableDownlines": true/false (in case of enable/disable status without bet type id),
		"custId": customer id,
		"uplineId": upline customer id,
		"userId": admin id or upline id,
		"productId": name of product, such as "livecasino", "casino", ...,
		"subAccId": sub account id,
		"language": Language key,
		"level": super/master/agent/member or 4/3/2/1
	}

2. Update: customersetting/enablestatus/productstatus/update?model={0}
	{0}: model that is generated into client
3. Example:
	customersetting/enablestatus/productstatus?custid=1176086&uplineid=1176083&userid=8&productid=keno&level=2&enable=true&enabledownlines=true&hasdownline=true&username=agent01&enableuplines=true