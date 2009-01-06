using System;
using System.IO;

using Do.Platform;
using Do.Universe;

namespace Quote
{
  public class QuoteTagItem : Item
  {
	public QuoteTagItem (string tag)
	{
	  Name = tag;
	  Description = tag;	  
	}

	public override string Name { get; private set; }

	public override string Description { get; private set; }
	
	public override string Icon { 
	  get { return "hash-icon.svg"; }
	}
  }
}	