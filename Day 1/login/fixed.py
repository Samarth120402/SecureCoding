from flask import Flask, request, render_template, redirect, url_for, session, flash
import sqlite3
from faker import Faker

app = Flask(__name__)
app.secret_key = 'supersecretkey'
fake = Faker()

# Initialize the database
def init_db():
    conn = sqlite3.connect('example.db')
    c = conn.cursor()
    c.execute('''CREATE TABLE IF NOT EXISTS users 
                 (id INTEGER PRIMARY KEY, username TEXT, password TEXT)''')
    c.execute("INSERT OR IGNORE INTO users (username, password) VALUES ('admin', 'admin123')")
  
    
    # Create a table for customers if it doesn't exist
    c.execute('''CREATE TABLE IF NOT EXISTS customers 
                 (id INTEGER PRIMARY KEY, name TEXT, email TEXT, phone TEXT)''')
    
    # Check if the customers table is empty and populate it with fake data
    c.execute("SELECT COUNT(*) FROM customers")
    if c.fetchone()[0] == 0:
        for _ in range(10):  # Add 10 random customers
            name = fake.name()
            email = fake.email()
            phone = fake.phone_number()
            c.execute("INSERT INTO customers (name, email, phone) VALUES (?, ?, ?)", (name, email, phone))
    
    conn.commit()
    conn.close()


def login(username, password):
    conn = sqlite3.connect('example.db')
    c = conn.cursor()
    query = f"SELECT * FROM users WHERE username=? AND password=?"
    c.execute(query,(username,password))
    result = c.fetchone()
    conn.close()
    return result



# Fetch all customers from the database
def get_customers():
    conn = sqlite3.connect('example.db')
    c = conn.cursor()
    c.execute("SELECT * FROM customers")
    customers = c.fetchall()
    conn.close()
    return customers

@app.route('/', methods=['GET', 'POST'])
def index():
    message = ""
    if request.method == 'POST':
        username = request.form['username']
        password = request.form['password']
       

        
        user = login(username, password)
        if user:
            session['username'] = user[1]
            return redirect(url_for('admin'))
        else:
            message = "Login Failed! Invalid credentials."
        

    return render_template('index.html', message=message)

@app.route('/admin')
def admin():
    if 'username' not in session:
        return redirect(url_for('index'))
    
    # Fetch customers from the database
    customers = get_customers()
    return render_template('admin.html', username=session['username'], customers=customers)

@app.route('/logout')
def logout():
    session.pop('username', None)
    return redirect(url_for('index'))

if __name__ == '__main__':
    init_db()
    app.run(debug=True)
