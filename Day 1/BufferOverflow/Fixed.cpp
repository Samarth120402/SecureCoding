/******************************************************************************

Welcome to GDB Online.
  GDB online is an online compiler and debugger tool for C, C++, Python, PHP, Ruby, 
  C#, OCaml, VB, Perl, Swift, Prolog, Javascript, Pascal, COBOL, HTML, CSS, JS
  Code, Compile, Run and Debug online from anywhere in world.

*******************************************************************************/
#include <cstdio>
#include <cstring>
#include <iostream>

const char *PASSWORD_FILE = "rictro";

int main()
{

  std::string input;
  char password[8];
  

  std::sscanf(PASSWORD_FILE, "%s", password);

  std::cout << "Enter password: ";
  std::cin >> input;

  // Debug prints:
  // std::cout << "Address of input: " << &input << "\n";
  // std::cout << "Address of password: " << &password << "\n";
  // std::cout << "Input: " << input << "\n";
  // std::cout << "Password: " << password << "\n";

  if (std::strncmp(password, input.c_str(), 8) == 0)
    std::cout << "Access granted\n";
  else
    std::cout << "Access denied\n";

  return 0;
}
