void Mutation()
    {
        int[] children = { 50, 35, 100, 200, 10 }; //Gene values in int 
        string[] genes = new String[children.Length];
        System.Random r = new System.Random();
        for (int i = 0; i < children.Length; i++)
        {

            StringBuilder temp = new StringBuilder(Convert.ToString(children[i], 2));
            Debug.Log(i.ToString() + "th " + "Gene Before Mutation :" + temp);
            int r1 = r.Next(temp.Length);
            int r2 = r.Next(temp.Length);

            if (temp[r1] == '0')
            {
                temp[r1] = '1';
            }
            else
            {
                temp[r1] = '0';
            }
            if (temp[r2] == '0')
            {
                temp[r2] = '1';
            }
            else
            {
                temp[r2] = '0';
            }
            genes[i] = temp.ToString();
            Debug.Log(i.ToString() + "th " + "Gene After Mutation :" + genes[i]);
        }

    }
