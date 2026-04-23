using System;
using System.Collections.Generic;

[Serializable]
public class CustomerReviewTextDataTable
{
	public ECustomerReviewType customerReviewType;

	public List<string> bad_TextList;

	public List<string> normal_TextList;

	public List<string> good_TextList;
}
