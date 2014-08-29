var TestR = {
	properties: [
		"selected", "textContent", "className", "checked", "readOnly", "multiple", "value", "nodeType", "innerText",
		"innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex", "id", "name", "tagName"
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
			if (element.id === undefined || element.id === '') {
				element.id = 'testR-' + id++;
			}
		}

		for (i = 0; i < allElements.length; i++) {
			element = allElements[i];
			
			var item = {
				id: element.id,
				parentId: element.parentNode.id,
				name: element.name || '',
				tagName: (element.tagName || '').toLowerCase(),
				attributes: [],
			};

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

			TestR.properties.forEach(function (name) {
				if (element[name] !== null && element[name] !== undefined) {
					item.attributes.push(name);
					if (typeof element[name] === 'string') {
						item.attributes.push(element[name]);
					} else {
						item.attributes.push(JSON.stringify(element[name]));
					}
				}
			});

			response.push(item);
		}
		
		return response;
	},
	getElementValue: function (id, name) {
		var element = document.getElementById(id);
		if (element === undefined || element === null) {
			return '';
		}

		var value = TestR.properties.contains(name) ? element[name] : element.attributes[name];
		if (value !== null && value !== undefined) {
			return value.toString();
		}

		return '';
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
	}
};

Array.prototype.contains = function (obj) {
	for (var i = 0; i < this.length; i++) {
		if (this[i] === obj) {
			return true;
		}
	}

	return false;
};
console.log("TestR injected...");