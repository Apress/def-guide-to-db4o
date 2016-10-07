class GroovyPerson {
	firstName
	lastName
	address1
	city
	province
	postcode
}

p = new GroovyPerson(firstName:'Ian', lastName:'Darwin', province:'Ontario')

println "Hello ${p.firstName} ${p.lastName}"