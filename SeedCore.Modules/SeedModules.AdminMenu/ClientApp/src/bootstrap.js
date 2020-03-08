import React from 'react';
import ReactDOM from 'react-dom';
import Navigation from './Navigation';

const bootstrap = config => {
  const documentReady = (function() {
    //这个函数返回whenReady()函数
    let funcs = []; //当获得事件时，要运行的函数
    let ready = false; //当触发事件处理程序时,切换为true

    //当文档就绪时,调用事件处理程序
    function handler(e) {
      if (ready) return; //确保事件处理程序只完整运行一次

      //如果发生onreadystatechange事件，但其状态不是complete的话,那么文档尚未准备好
      if (
        e.type === 'onreadystatechange' &&
        document.readyState !== 'complete'
      ) {
        return;
      }

      //运行所有注册函数
      //注意每次都要计算funcs.length
      //以防这些函数的调用可能会导致注册更多的函数
      for (let i = 0; i < funcs.length; i++) {
        funcs[i].call(document);
      }
      //事件处理函数完整执行,切换ready状态, 并移除所有函数
      ready = true;
      funcs = [];
    }
    //为接收到的任何事件注册处理程序
    if (document.addEventListener) {
      document.addEventListener('DOMContentLoaded', handler, false);
      document.addEventListener('readystatechange', handler, false); //IE9+
      window.addEventListener('load', handler, false);
    } else if (document.attachEvent) {
      document.attachEvent('onreadystatechange', handler);
      window.attachEvent('onload', handler);
    }
    //返回函数
    return function(fn) {
      if (ready) {
        fn.call(document);
      } else {
        funcs.push(fn);
      }
    };
  })();

  documentReady(function() {
    const content = [];
    document.body.childNodes.forEach(node => {
      content.push(node);
    });
    const root = document.createElement('div');
    root.className = 'navi';
    ReactDOM.render(
      <Navigation body={content} site={config.site} />,
      document.body.appendChild(root)
    );
  });
};

export default bootstrap;
