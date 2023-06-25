function getJsonValue(value)
{
  var result;
  var type = typeof value;
  switch(type)
  {
    case 'string':
    result = '"' + value + '"';
    break;
    case 'boolean':
    result = value;
    break;
    case 'number':
    result = value;
    break;
    case 'symbol':
    result = "'" + value + "'";
    break;
    case 'object':
    result = JSON.stringify(value);
    break;
  }

  return result;
}