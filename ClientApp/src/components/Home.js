import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <h1>Вітаю!</h1>
                <p>Ласкаво просимо до програми, створеної за допомогою:</p>
                <ul>
                    <li><a href='https://get.asp.net/'>ASP.NET Core</a> і <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> для розробки серверної сторони моудля</li>
                    <li><a href='https://facebook.github.io/react/'>React</a> для розробки клієнтської сторони модуля</li>
                    <li><a href='http://getbootstrap.com/'>Bootstrap</a> для стилізації модуля</li>
                </ul>  
            </div>
        );
    }
}
