void Crossover()
    {
        int p1 = 20;
        int p2 = 40;
        string p1_ = Convert.ToString(p1, 2);
        string p2_ = Convert.ToString(p2, 2);
        p1_ = p1_.PadLeft(8, '0');
        p2_ = p2_.PadLeft(8, '0');
        string crossover_child = p1_.Substring(0, 4) + p2_.Substring(4, 4);
        Debug.Log("Parent 1:" + p1_);
        Debug.Log("Parent 2:" + p2_);
        Debug.Log("Crossover:" + crossover_child);    
    }
