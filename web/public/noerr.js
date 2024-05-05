// https://stackoverflow.com/a/75848002/3530727

self.addEventListener('fetch', e => {
  return e.respondWith(
    fetch(e.request).then(orig => {
      return orig.status < 400 ? orig : new Response(null, {
        status: 202,
        statusText: 'Accepted',
        headers: new Headers({
          'Status': orig.status,
          'StatusText': orig.statusText
        })
      });
    })
  );
});
