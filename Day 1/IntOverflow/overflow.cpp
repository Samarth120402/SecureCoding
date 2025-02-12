#include <iostream>
#include <limits>

int add(int a, int b) {
    return a + b;
}

int main() {
    int x,y;
    std::cout << "Enter the first integer (x): ";
    std::cin >> x;
    std::cout << "Enter the second integer (y): ";
    std::cin >> y;
    int result = add(x, y);  
    std::cout << "Result: " << result << std::endl;
    return 0;
}
