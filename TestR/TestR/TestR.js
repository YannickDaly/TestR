var TestR = {
	triggerEvent: function (element, eventName, values) {
		var eventObj = document.createEventObject
			? document.createEventObject()
			: document.createEvent("Events");

		if (eventObj.initEvent) {
			eventObj.initEvent(eventName, true, true);
		}

		for (var i = 0; i < values.length; i++) {
			console.log(values[i].key + " : " + values[i].value);
			eventObj[values[i].key] = values[i].value;
		}

		//eventObj.keyCode = keyCode;
		//eventObj.which = keyCode;

		element.dispatchEvent
			? element.dispatchEvent(eventObj)
			: element.fireEvent("on" + eventName, eventObj);
	},
};