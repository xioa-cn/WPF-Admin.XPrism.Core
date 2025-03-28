namespace WPFAdmin.Test;

public class MinimumCostTest {
    public long MinimumCost(string s) {
        int n = s.Length;
        long ans = 0;
        
        // 遍历每个位置（除了首尾）
        for (int i = 1; i < n; i++) {
            // 如果当前位置和前一个位置的字符不同
            // 则需要进行反转操作
            if (s[i] != s[i-1]) {
                // 取较小的成本：从开头反转到i-1，或从i反转到结尾
                ans += Math.Min(i, n-i);
            }
        }
        
        return ans;
    }
}