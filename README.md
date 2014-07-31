StencilBox
==========

StencilBox is a very (very) simple reflection-based templating engine.

## Nuget Installation

	PM> Install-Package StencilBox

## Simple "box" syntax for templates

	[~NameOfProperty]
	
Let's say we have the following c# model

	public class MyModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}

We can write a `template` for this model as a simple string; perhaps it contains

	Hello, [~Name]! \nYou must be very [~Description].


The code to generate the output is as follows:
	
	var model = new MyModel { Name = "StencilBox", Description = "convenient" };
	var output = Stencil.Apply(template, model);
	
Thus, `output` would be

	Hello, StencilBox! 
	You must be very convenient.


## Some Extras

StencilBox has a few other minor tricks up its sleeve.

You can augment the model's properties by also passing in a dictionary of key/value pairs. You can insert the Values into the template by referencing the Key as follows

	// For manual replacements (key token is replaced with value):	[m~key]
	

There are also a small list of modifiers that can be useful. A couple of these are:

 * `codify` - codifies the output (removes spaces and non-alphanumeric characters, etc)
 * `lowercase` - lowercases the output
 * `uppercase` - uppercases the output

Usage of these modifiers is as follows:

	[~NameOfProperty:optionalmodifier]
	
Thus, to uppercase our Description property, it would be

	Hello, [~Name]! \nYou must be very [~Description:uppercase].
