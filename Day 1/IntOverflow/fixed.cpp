#include <iostream>
#include <limits>
 
bool safe_add(int a, int b, int &result) {
    if((b>0 && a> std::numeric_limits<int>::max()-b) || (b<0 && a< std::numeric_limits<int>::min()-b)) 
    {
        return false;
    }
    result =a+b;
    return true;
}
 
int main() {
    //int x = 2000000000000;
    //int y = 1500000000000;
    //int result;
    int x,y,result;
    std::cout << "Enter the first integer (x): ";
    std::cin >> x;
    std::cout << "Enter the second integer (y): ";
    std::cin >> y;
    if(safe_add(x,y,result))
    {
        std::cout << "Result: "<< result << std::endl;
    }
    else
    {
        std::cout<<"Integer Overflow";
    }
    return 0;

}
