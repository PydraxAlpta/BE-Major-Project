public int[] TournamentSelection(int[] children, int[] fitnessArray)
    {
        System.Random r = new System.Random();

        int min_child; //child with min fitness value 
        int max_child; //child with max fitness value 


        List<int> selected = new List<int>();

        var children_fitness = new Dictionary<int, int>();

        for (int index = 0; index < children.Length; index++)
        {
            children_fitness.Add(children[index], fitnessArray[index]);

        }

        max_child = children_fitness.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        min_child = children_fitness.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;


        int children_length; ;
        int count = 0;

        if (children.Length % 2 != 0) //if len(children) == odd; remove child with min fitness value to make len even
        {

            children_fitness.Remove(min_child);
            children = children.Where(val => val != min_child).ToArray();
        }

        children_length = children.Length; //length after removal of child with min fitness


        while (count != children_length)
        {
            count += 2;

            int[] random_child_values;
            random_child_values = children.OrderBy(x => r.Next()).Take(2).ToArray();

            if (children_fitness[random_child_values[0]] >= children_fitness[random_child_values[1]])
            {
                selected.Add(random_child_values[0]);
            }
            else
            {
                selected.Add(random_child_values[1]);
            }

            foreach (var value in random_child_values)
            {
                children = children.Where(val => val != value).ToArray();

            }

        }
        selected.Add(max_child); //add child with max fitness to make array even
        return selected.ToArray();

    } 
