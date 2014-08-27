var TestR = {
	properties: [
		"selected", "textContent", "className", "checked", "readOnly", "multiple", "value", "nodeType", "innerText",
		"innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex", "id", "name", "tagName", "class"
	],
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

		element.dispatchEvent
			? element.dispatchEvent(eventObj)
			: element.fireEvent("on" + eventName, eventObj);
	},
	getElements: function () {
		var response = [];
		var allElements = document.body.getElementsByTagName('*');
		var id = 1;


		for (var i = 0; i < allElements.length; i++) {
			var element = allElements[i];
			if (element.id === "undefined" || element.id === "") {
				element.id = "testR-" + id++;
			}

			var item = {
				id: element.id,
				name: element.name || "",
				tagName: element.tagName || "",
				attributes: [],
			}
			
			item.tagName = item.tagName.toLowerCase();
			
			for (var j = 0; j < element.attributes.length; j++) {
				var attribute = element.attributes[j];
				if (attribute.nodeName === undefined || attribute.nodeName.length <= 0) {
					continue;
				}
				if (TestR.properties.contains(attribute.nodeName)) {
					continue;
				}
				item.attributes.push(attribute.nodeName);
				item.attributes.push(attribute.value);
			}

			if (item.type) {
				item.attributes.push("type");
				item.attributes.push(item.type);
			}

			response.push(item);
		}

		return response;
	},
	getElementValue: function (id, name) {
		var element = document.getElementById(id);
		if (element === undefined || element === null) {
			return "";
		}

		if (TestR.properties.contains(name)) {
			return element[name].toString();
		} else {
			return element.attributes[name].toString();
		}
	},
	setElementValue: function (id, name, value) {
		var element = document.getElementById(id);
		if (element === undefined || element === null) {
			return;
		}

		if (TestR.properties.contains(name)) {
			element[name] = value;
		} else {
			element.setAttribute(name, value);
		}
	},
	execute: function (script) {
		try {
			document.executeResult = String(eval(script));
		} catch (error) {
			document.executeResult = error;
		};
	},
};

Array.prototype.contains = function (obj) {
	for (var i = 0; i < this.length; i++) {
		if (this[i] === obj) {
			return true;
		}
	}

	return false;
}

console.log("TestR injected...");