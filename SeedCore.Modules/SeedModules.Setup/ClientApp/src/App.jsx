import React, { useState } from 'react';

export default () => {
  const [text, setText] = useState('xxx');

  const doFetch = async () => {
    const response = await fetch('/SeedModules.Setup/Setup/Text');
    const json = await response.json();
    console.log(json);
  };

  return (
    <div>
      <h1>哈哈哈</h1>
      <input value={text} onChange={evt => setText(evt.target.value)}></input>
      <p>{text}</p>
      <button onClick={doFetch}>fetch</button>
    </div>
  );
};
